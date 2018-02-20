namespace Composite

open System
open System.Collections.Generic
open System.Linq;

open FSharp.Core

open DataTypes
open Transforms

module C =

    let ToForest (inputComposite: 'a Composite) =
        Enumerable.AsEnumerable(toForest inputComposite)

    let ToFlat (inputComposite: 'a Composite) =
        Enumerable.AsEnumerable(toFlat inputComposite)

    let Ana (scn: IEnumerable<Func<'a, 'b>>) (obj: 'a Composite ) =
        let scenario = scn |> List.ofSeq
                           |> List.map (fun x -> x.Invoke)
        ana scenario obj

    let Composite (inputSequence: IEnumerable<'a Composite>) =
        Composite inputSequence

    let Value value =
        Value value        