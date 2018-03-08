namespace Composite

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
    /// Unfolds the source Composite according to the specified array of steps.
    /// </summary>
    /// <param name="source">
    /// The source composite.
    /// </param>
    /// <param name="scenario">
    /// An array element-to-enumerable transformation rules.
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
    static member inline Ana (source: 'T Composite) (scenario: CheckUnfoldRule<'T>[]) =
        if scenario |> isNull then nullArg "scenario"
        if scenario |> Array.isEmpty then invalidArg "scenario" "Unfold scenario must contain at least one rule."

        ana (scenario |> Array.map (fun x -> x.CheckFunction.Invoke, x.UnfoldFunction.Invoke)) source

module C =

    let ToForest (inputComposite: 'a Composite) =
        Enumerable.AsEnumerable(toForest inputComposite)

    let ToFlat (inputComposite: 'a Composite) =
        Enumerable.AsEnumerable(toFlat inputComposite)

    let Composite (inputSequence: IEnumerable<'a Composite>) =
        Composite inputSequence

    let Value value =
        Value value        