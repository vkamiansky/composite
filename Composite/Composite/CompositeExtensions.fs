namespace Composite

open System
open System.Collections.Generic
open System.Linq;
open System.Runtime.CompilerServices


open FSharp.Core

open DataTypes
open Transforms

[<Extension>]
type CompositeExtensions () =

    [<Extension>]
    /// <summary>
    /// Unfolds the source Composite according to the
    /// specified array of steps.
    /// </summary>
    /// <param name="source">
    /// The source composite.
    /// </param>
    /// <param name="scenario">
    /// The unfold scenario comprising steps as
    /// rules of element-to-enumerable transformation.
    /// </param>
    /// <typeparam name="T">
    /// The type of payload elements in the composite.
    /// </typeparam>
    /// <returns>
    /// An unfolded composite.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <c>scenario</c> is empty.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when <c>scenario</c> is null.
    /// </exception>
    static member inline Ana (source: 'T Composite) (scenario: Func<'T, IEnumerable<'T>>[]) =
        if scenario |> isNull then nullArg "scenario"
        if scenario |> Array.isEmpty then invalidArg "scenario" "Unfold scenario must contain at least one step."

        ana (scenario |> Array.map (fun x -> x.Invoke)) source

module C =

    let ToForest (inputComposite: 'a Composite) =
        Enumerable.AsEnumerable(toForest inputComposite)

    let ToFlat (inputComposite: 'a Composite) =
        Enumerable.AsEnumerable(toFlat inputComposite)

    let Composite (inputSequence: IEnumerable<'a Composite>) =
        Composite inputSequence

    let Value value =
        Value value        