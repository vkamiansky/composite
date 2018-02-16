namespace Composite

open System
open System.Collections.Generic
open System.Linq;

open FSharp.Core

open DataTypes
open Transforms

module C =

    type PickTransformPair<'a>(pickFuctions, transformFunction) =

        member this.PickFuctions: IEnumerable<Func<'a, 'b>> when 'b:null = pickFuctions

        member this.TransformFunction: Func<IEnumerable<'b>, IEnumerable<'c>> when 'b:null = transformFunction

    let ToForest (inputComposite: 'a Composite) =
        Enumerable.AsEnumerable<'a Composite>(toForest inputComposite)

    let ToFlat (inputComposite: 'a Composite) =
        Enumerable.AsEnumerable<'a>(toFlat inputComposite)

    let Ana (scn: IEnumerable<Func<'a, 'b>>) (obj: 'a Composite ) =

        let scenario = scn |> List.ofSeq
                           |> List.map (fun x -> x.Invoke)
        ana scenario obj

    let Composite (inputSequence: IEnumerable<'a>) =
        Composite inputSequence

    let Value value =
        Value value

    let Cata (scenario: IEnumerable<PickTransformPair<'a>>)
             (inputSequence: IEnumerable<'a>) =

        let fs_sf (sf: IEnumerable<Func<'a, 'b>> when 'b:null) =
            sf |> List.ofSeq |> List.map (fun x -> (fun a -> match x.Invoke(a) with
                                                             | null -> None
                                                             | y -> Some y ))

        let fs_tf (tf: Func<IEnumerable<'b>, IEnumerable<'c>> when 'b:null) =
           fun (b: 'b option list) -> tf.Invoke(b |> Seq.ofList 
                                                  |> Seq.map(function | Some x -> x | None -> null) )|> List.ofSeq

        
        cata (scenario |> List.ofSeq |> List.map (fun x -> fs_sf x.PickFuctions, fs_tf x.TransformFunction)) inputSequence
        