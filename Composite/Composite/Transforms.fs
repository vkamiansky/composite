namespace Composite

open Composite.DataTypes

module Transforms =

    let private toComposite obj =
        match Seq.tryHead obj with
        | None -> failwith "Empty data sequence will not lead to a meaningful Composite instance."
        | Some x -> let obj2 = Seq.tail obj
                    match Seq.tryHead obj2 with
                    | None -> Value x
                    | Some y -> Composite(seq {
                                              yield Value x
                                              yield Value y
                                              yield! Seq.map Value (Seq.tail obj2)
                                           })

    let rec ana scn obj =
        match scn with
        | [] -> obj
        | f :: scn_tail -> match obj with
                           | Value x -> ana scn_tail (toComposite(f x))
                           | Composite x -> Composite(Seq.map (ana scn) x)
                           
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

    let cata scn lst =
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
               | Some head -> match update_results f (Seq.head x) with
                              | (frames_new, results_new) -> yield! results_new
                                                             yield! get_results frames_new (Seq.tail x)
              }

        get_results frames_init lst