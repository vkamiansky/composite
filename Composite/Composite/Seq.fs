namespace Composite

open FSharp.Core
open System.Threading

[<RequireQualifiedAccess>]
module Seq =

    ///<summary>Views the given <c>Composite</c> as a sequence.</summary>
    ///<param name="source">The input <c>Composite</c>.</param>
    let rec ofComp (source: 'T Composite) =
       seq {
            match source with
               | Value x -> yield x
               | Composite x -> yield! x |> Seq.collect ofComp
        }

    ///<summary>Views the input sequence as a function fetching its next element.</summary>
    ///<param name="source">The input sequence.</param>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>source</c> is null.</exception>
    let fetchElementFunction (source: seq<'T>) =
        if source |> isNull then nullArg "source"

        let enumerator = source.GetEnumerator ()
        fun () -> if enumerator.MoveNext ()
                  then Some enumerator.Current
                  else None

    ///<summary>Divides the input sequence into multiple non-intersecting sequences.</summary>
    ///<param name="numParts">The desired number of partitions.</param>
    ///<param name="source">The input sequence.</param>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>source</c> is null.</exception>
    ///<exception cref="System.ArgumentException">Thrown when <c>numParts</c> is less than 1.</exception>
    let partition (numParts: int) (source: seq<'T>) =
        if source |> isNull then nullArg "source"
        if numParts < 1 then invalidArg "numParts" "Number of partitions must be positive."

        // An object used for thread synchronization.
        let lockObj = new obj ()

        let fetchElem = source |> fetchElementFunction

        // We create a thread-safe version
        // of fetchElem. This we need in order to
        // be able to process multiple partitions in parallel. 
        let fetchElemSafe () =
            Monitor.Enter lockObj
            let res = fetchElem ()
            Monitor.Exit lockObj
            res

        let rec getSeq () = 
            seq {
                    match fetchElemSafe () with
                    | Some element -> yield element
                                      yield! getSeq ()
                    | None -> yield! []
            }

        Array.init numParts (fun _ -> getSeq ())

    ///<summary>Divides the input sequence into chunks of as many elements as possible their total weight not exceeding <c>chunkWeight</c> but not than one element per batch.</summary>
    ///<param name="chunkWeight">The preferred maximum weight of each chunk.</param>
    ///<param name="weigh">The function used to weigh elements.</param>
    ///<param name="source">The input sequence.</param>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>source</c> is null.</exception>
    ///<exception cref="System.ArgumentException">Thrown when <c>chunkWeight</c> is less than 1.</exception>
    let chunkByWeight chunkWeight weigh (source: seq<'T>) =
        if source |> isNull then nullArg "source"
        if chunkWeight < 1 then invalidArg "chunkWeight" "Preferred maximum chunk weight must be positive."

        // We walk through a sequence and assemble a chunk.
        // If we have an extra element we put into
        // the next chunk and return the weight of the next chunk
        // as well so we don't need to calculate it again.
        let rec getNewFrame roomLeft fetchElem frame = 
            let (chunk, nextChunk, nextChunkWeight) = frame
            fetchElem ()
            |> function
               | None -> frame
               | Some head -> let size = weigh head
                              let roomLeftNew = roomLeft - size
                              chunk
                              |> function
                                 | [||] -> getNewFrame roomLeftNew fetchElem ([|head|], nextChunk, nextChunkWeight)
                                 | _ -> if roomLeftNew < 0
                                        then chunk, [|head|], size
                                        else getNewFrame roomLeftNew fetchElem ([|head|] |> Array.append chunk, nextChunk, nextChunkWeight)

        let rec getChunks curChunk curChunkWeight fetchElem =
            seq {
                    match getNewFrame (chunkWeight-curChunkWeight) fetchElem (curChunk, [||], 0) with
                    | [||], _, _ -> yield! []
                    | chunk, nextChunk, nextChunkWeight -> yield chunk
                                                           yield! getChunks nextChunk nextChunkWeight fetchElem
            }

        source |> fetchElementFunction |> getChunks [||] 0

    ///<summary>Finds elements in the input sequence to populate parameter arrays, applies the respective transform functions to populated arrays, and concatenates the results.</summary>
    ///<param name="rules">An array of check and transform rules. Each rule has an array of check functions (i-th one checking if the element is fit to fill the i-th position in the parameters' array), and a function transforming the parameters array into a result sequence.</param>
    ///<param name="source">The input sequence.</param>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>rules</c> or <c>source</c> is null.</exception>
    ///<exception cref="System.ArgumentException">Thrown when <c>rules</c> contains rules with no check functions.</exception>
    ///<typeparam name="TIn">The type of elements in the input sequence.</typeparam>
    ///<typeparam name="TOut">The type of elements in the output sequence.</typeparam>
    let accumulateCollect (rules: (('TIn -> bool)[] * ('TIn[] -> seq<'TOut>))[]) (source: seq<'TIn>) =
        if source |> isNull then nullArg "source"
        if rules |> isNull then nullArg "rules"
        if rules |> Array.exists (fun (funcs, _) -> funcs |> isNull || funcs |> Array.isEmpty) then invalidArg "rules" "A check-transform rule must contain at least one check function."

        // We initialize the frames
        // Each frame contains:
        // 1) an array of check functions;
        //    The i-th function returns whether the seq element is fit to be i-th parameter
        //    of the transform function
        // 2) a transform function.
        //    Once all the parameters are filled, the transform function will use them to
        //    calculate its results and yield them to the output sequence. 
        let framesInitial = rules |> Array.map (fun (checkFuncs, transform) -> checkFuncs, transform, (fun _ -> None) |> Array.init (checkFuncs |> Array.length))

        // This function will examine the parameters accumulator
        // for unfilled parameters and whenever it finds one
        // it will do the following:
        // - If the given object (obj) passes the check function for
        //   the given parameter, use it as a new parameter value;
        // - If the check is not passed, the parameter is skipped.
        let checkAndUpdateAcc checkFuncs acc obj =
            Array.map2 (fun f p -> (f, p) |> function | f, None -> (if (f obj) then Some obj else None) | _, p -> p) checkFuncs acc

        // This function will use the frames to update their accumulators based on
        // the value of obj and return frames with unfilled parameter accumulators
        // and results produced by applying the respective transform functions
        // to filled parameter accumulators.
        let rec getFramesAndResults framesAndResults obj =
            framesAndResults |> function 
                                | [||], r -> [||], r
                                | f, r -> f |> Array.head
                                            |> function 
                                               | (checkFuncs, transformFunc, acc) ->
                                                    let accNew = checkAndUpdateAcc checkFuncs acc obj
                                                    let (framesNew, resultsNew) = getFramesAndResults (f |> Array.tail, r) obj
                                                    if accNew |> Array.exists (function | None -> true | _ -> false) then
                                                        [|checkFuncs, transformFunc, accNew|] |> Array.append framesNew, resultsNew
                                                        else
                                                        let transformed = accNew |> Array.choose id |> transformFunc 
                                                        framesNew, resultsNew |> Seq.append (transformed)

        // This function gets frames and results for each object in the input sequence
        // and yields the results to the output sequence
        let rec getResults frames getElem =
            seq {
                    match frames with
                    | [||] -> yield! []
                    | f ->
                       match getElem () with
                       | None -> yield! []
                       | Some head -> match getFramesAndResults (f, Seq.empty) head with
                                      | (framesNew, resultsNew) ->  yield! resultsNew
                                                                    yield! getResults framesNew getElem
            }

        source |> fetchElementFunction |> getResults framesInitial