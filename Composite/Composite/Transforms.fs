namespace Composite

open System.Threading
open Composite.DataTypes

module Transforms =

    let toForest inComp =
        match inComp with
        | Composite x -> x
        | x -> seq { yield x }

    let rec toFlat inComp =
       seq {
            match inComp with
               | Value x -> yield x
               | Composite x -> yield! x |> Seq.collect toFlat
        }

    let toPartitioned numParts inSeq =
        let lockObj = new obj ()

        let mutable restOfSeq = inSeq

        let getNext () =
            Monitor.Enter lockObj
            let res = restOfSeq |> Seq.tryHead
            restOfSeq <- (restOfSeq |> Seq.tail)
            Monitor.Exit lockObj
            res

        let getSeq () = 
            seq {
                    let mutable cur = getNext ()
                    while cur |> Option.isSome do
                        yield cur.Value
                        cur <- getNext ()
            }

        Array.init numParts (fun _ -> getSeq ())

    let toPaged pageSize inSeq =
        let rec getPageAndRest numRemains pageAndRest = 
            match numRemains > 0, pageAndRest with
            | true, (p, r) -> match r |> Seq.tryHead with
                                | Some h -> getPageAndRest (numRemains-1) (Array.append p [|h|], r |> Seq.tail)
                                | None -> pageAndRest
            | false, pr -> pr

        let rec getPages inSeq2 =
            seq {
                    match getPageAndRest pageSize ([||], inSeq2) with
                    | [||], _ -> yield! []
                    | p, r -> yield p
                              yield! getPages r
            }
        getPages inSeq

    let toBatched batchSize getElemSize inSeq =
        // Here we extract a batch from an input sequence.
        // We do this recursively, passing the amount of remaining
        // space in batch as a parameter, plus a tuple comprising
        // the batch under construction and the remaining sequence
        // we have to walk through.
        let rec getBatchAndRest roomLeft batchAndRest = 
            let (batch, rest) = batchAndRest
            rest
            |> Seq.tryHead
            |> function
               | Some head -> let size = getElemSize head
                              let roomLeftNew = roomLeft - size
                              batch
                              |> function
                                 | [||] -> getBatchAndRest roomLeftNew ([|head|], rest |> Seq.tail)
                                 | _ -> if roomLeftNew < 0
                                        then batchAndRest
                                        else getBatchAndRest roomLeftNew ([|head|] |> Array.append batch, rest |> Seq.tail)
               | None -> batchAndRest

        let rec getBatches inSeq2 =
            seq {
                    match getBatchAndRest batchSize ([||], inSeq2) with
                    | [||], _ -> yield! []
                    | p, r -> yield p
                              yield! getBatches r
            }

        getBatches inSeq

    let private toComposite inSeq =
        match Seq.tryHead inSeq with
        | None -> failwith "Empty data sequence will not lead to a meaningful Composite instance."
        | Some x -> let inSeqTail = Seq.tail inSeq
                    match Seq.tryHead inSeqTail with
                    | None -> Value x
                    | Some y -> Composite(seq {
                                              yield Value x
                                              yield Value y
                                              yield! (Seq.tail inSeqTail) |> Seq.map Value
                                           })

    let rec ana scn inComp =
        match scn with
        | [] -> inComp
        | f :: scn_tail -> match inComp with
                           | Value x -> ana scn_tail (toComposite(f x))
                           | Composite x -> Composite(x |> Seq.map (ana scn))                      

    let cata scn inSeq =
        if inSeq |> isNull then nullArg "inSeq"
        if scn |> isNull then nullArg "scn"
        if scn |> Array.exists (fun (funcs, _) -> funcs |> isNull || funcs |> Array.isEmpty) then invalidArg "scn" "A check-transform rule must contain at least one check function."

        // We initialize the frames
        // Each frame contains:
        // 1) an array of check functions;
        //    The i-th function returns whether the seq element is fit to be i-th parameter
        //    of the transform function
        // 2) a transform function.
        //    Once all the parameters are filled, the transform function will use them to
        //    calculate its results and yield them to the output sequence. 
        let framesInitial = scn |> Array.map (fun (checkFuncs, transform) -> checkFuncs, transform, (fun _ -> None) |> Array.init (checkFuncs |> Array.length))

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
        let rec getResults frames objs =
            seq {
                    match frames, objs with
                    | [||], _ -> yield! []
                    | f, x ->
                       match Seq.tryHead x with
                       | None -> yield! Seq.empty
                       | Some head -> match getFramesAndResults (f, Seq.empty) head with
                                      | (framesNew, resultsNew) ->  yield! resultsNew
                                                                    yield! getResults framesNew (Seq.tail x)
            }

        getResults framesInitial inSeq