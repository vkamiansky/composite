namespace Composite

open System.Threading

open DataTypes

module Transforms =

    let toGetElement (inSeq: seq<_>) = 
        let enumerator = inSeq.GetEnumerator ()
        fun () -> if enumerator.MoveNext ()
                  then Some enumerator.Current
                  else None

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

        let getElem = inSeq |> toGetElement

        // We create a thread-safe version
        // of getElem. We need it in order to
        // make partitions work in parallel consistently. 
        let getElemSafe () =
            Monitor.Enter lockObj
            let res = getElem ()
            Monitor.Exit lockObj
            res

        let rec getSeq () = 
            seq {
                    match getElemSafe () with
                    | Some element -> yield element
                                      yield! getSeq ()
                    | None -> yield! []
            }

        Array.init numParts (fun _ -> getSeq ())

    let toBatched batchSize getElemSize inSeq =
        // We walk through a sequence and assemble a batch
        // of elements. If we have an extra element we put into
        // the next batch and pass the size of the next batch
        // as well so we don't need to calculate it again.
        let rec getNewFrame roomLeft getElem frame = 
            let (batch, batchNext, batchNextSize) = frame
            getElem ()
            |> function
               | None -> frame
               | Some head -> let size = getElemSize head
                              let roomLeftNew = roomLeft - size
                              batch
                              |> function
                                 | [||] -> getNewFrame roomLeftNew getElem ([|head|], batchNext, batchNextSize)
                                 | _ -> if roomLeftNew < 0
                                        then batch, [|head|], size
                                        else getNewFrame roomLeftNew getElem ([|head|] |> Array.append batch, batchNext, batchNextSize)

        let rec getBatches curBatch curBatchSize getElem =
            seq {
                    match getNewFrame (batchSize-curBatchSize) getElem (curBatch, [||], 0) with
                    | [||], _, _ -> yield! []
                    | b, bNext, bNextSize -> yield b
                                             yield! getBatches bNext bNextSize getElem
            }

        inSeq |> toGetElement |> getBatches [||] 0

    let ana scn inComp =
        if scn |> isNull then nullArg "scn"
        if scn |> Array.isEmpty then invalidArg "scn" "Unfold scenario must contain at least one step."

        // Applies a check/unfold rule to the given object.
        // If the object passes the check function it is unfolded
        // through the use of the unfold function, the results are wrapped
        // into Values and their enumerable - into a Composite.
        // If the object does not pass the check function it
        // is returned as is wrapped in a Value.
        let applyRuleAndWrapResult rule obj =
            let (checkFunc, unfoldFunc) = rule
            obj
            |> checkFunc
            |> function
                | true -> obj |> unfoldFunc |> Seq.map Value |> Composite
                | _ -> obj |> Value

        // We pass through a sequence and apply the unfold
        // steps to each object. Each step produces a sequence of
        // results we pack into a new composite of values.
        let rec getResults steps comp =
            steps
            |> Array.tryHead
            |> function
                | None -> comp
                | Some step -> comp
                                |> function
                                   | Value x -> getResults (steps |> Array.tail) (x |> applyRuleAndWrapResult step)
                                   | Composite x -> if x |> isNull then failwith "Composite sequence must not be null."
                                                    x |> Seq.map (getResults steps) |> Composite
        
        getResults scn inComp


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

        inSeq |> toGetElement |> getResults framesInitial