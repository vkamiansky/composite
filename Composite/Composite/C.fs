namespace Composite

open System
open System.Collections.Generic

open FSharp.Core

open DataTypes
open Transforms

module C =
    let Ana (scn: IEnumerable<Func<'a, 'b>>) (obj: 'a Composite ) =

        let scenario = scn |> List.ofSeq
                           |> List.map (fun x -> x.Invoke )
        ana scenario obj
        
    let Value (obj:'a) =
        Value obj

    let Composite (obj: IEnumerable<'a>) =
        Composite (obj |> Seq.map Composite.Value)

    type FindTransformPair<'a>(findFuctions, transformFunction) =

        member this.SearchFuctions: IEnumerable<Func<'a, 'b>> when 'b:null = findFuctions

        member this.TransformFunction: Func<IEnumerable<'b>, IEnumerable<'c>> when 'b:null = transformFunction

    let NullableFuncToOption (func : ('a -> Nullable<_>)) funcInput = 
        let result = func funcInput
        if result.HasValue 
        then Some result.Value
        else None

    let Cata (scn: IEnumerable<FindTransformPair<'a>>)
             (lst: IEnumerable<'a>) =

        let fs_sf (sf: IEnumerable<Func<'a, 'b>> when 'b:null) =
            sf |> List.ofSeq |> List.map (fun x -> (fun a -> match x.Invoke(a) with
                                                             | null -> None
                                                             | y -> Some y ))

        let fs_tf (tf: Func<IEnumerable<'b>, IEnumerable<'c>> when 'b:null) =
           fun (b: 'b option list) -> tf.Invoke(b |> Seq.ofList 
                                                  |> Seq.map(function | Some x -> x | None -> null) )|> List.ofSeq

        
        cata (scn |> List.ofSeq |> List.map (fun x -> fs_sf x.SearchFuctions, fs_tf x.TransformFunction)) lst
        