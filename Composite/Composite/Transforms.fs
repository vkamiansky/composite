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
                        yield cur |> (function | Some x -> x | None -> failwith "Unexpected situation in enumeration.")
                        cur <- getNext ()
                }

        Array.init numParts (fun _ -> getSeq ())

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
                           
//    let v f obj =
//        match f obj with
//        | Nil -> failwith "Empty data sequence is an invalid binding result."
//        | Cons(x, Nil) -> if x = obj then ll x else [obj; x] |> LazyList.ofList
//        | x -> LazyList.cons obj x

    let private update_acc check_funcs acc obj =
        let mapping f acc = 
            match f, acc with
            | f, None -> f obj
            | _, res -> res

        List.map2 mapping check_funcs acc

    let private update_results frames obj =
        let pre_results = frames
                          |> List.map (function
                                       | (check_funcs, transform, acc) ->
                                            (let acc_new = update_acc check_funcs acc obj
                                             match transform acc_new with
                                             | [] -> (Some (check_funcs, transform, acc_new), [])
                                             | r -> (None, r)))

        pre_results |> List.choose (function | (f, _) -> f),
        pre_results |> Seq.collect (function | (_, r) -> r)

    let cata scn inSeq =
        let frames_init = scn |> List.map (function 
                                           | (check_funcs, transform) -> (check_funcs, transform, check_funcs
                                                                                                  |> List.map (fun _ -> None)))
        let rec get_results frames objs =
            match frames, objs with
            | [], _ -> Seq.empty
            | f, x ->
              seq {
               match Seq.tryHead x with
               | None -> yield! Seq.empty
               | Some head -> match update_results f head with
                              | (frames_new, results_new) -> yield! results_new
                                                             yield! get_results frames_new (Seq.tail x)
              }

        get_results frames_init inSeq