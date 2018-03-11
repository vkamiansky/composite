namespace Composite

open System.Linq;
open System.Runtime.CompilerServices

open FSharp.Core

[<Extension>]
type CompositeExtensions () =

    [<Extension>]
    ///<summary>Unfolds the source Composite according to the specified array of steps.</summary>
    ///<param name="source">The source composite.</param>
    ///<param name="scenario">An array element-to-enumerable transformation rules.</param>
    ///<typeparam name="T">The type of payload elements in the composite.</typeparam>
    ///<returns>An unfolded composite.</returns>
    ///<exception cref="System.ArgumentException">Thrown when <c>scenario</c> is empty.</exception>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>scenario</c> is null.</exception>
    static member inline Ana (source: 'T Composite) (scenario: CheckUnfoldRule<'T>[]) =
        if scenario |> isNull then nullArg "scenario"
        if scenario |> Array.isEmpty then invalidArg "scenario" "Unfold scenario must contain at least one rule."

        Comp.unfold (scenario |> Array.map (fun x -> x.CheckFunction.Invoke, x.UnfoldFunction.Invoke)) source

    [<Extension>]
    ///<summary>Returns the inner forest of a composite <c>source</c> or a single-element enumerable with a value <c>source</c>.</summary>
    ///<param name="source">The input <c>Composite</c>.</param>
    ///<typeparam name="T">The type of payload elements in the <c>Composite</c>.</typeparam>
    static member inline ToForest (source: 'T Composite) =
        Enumerable.AsEnumerable(Comp.components source)

    [<Extension>]
    ///<summary>Views the given <c>Composite</c> as an enumerable.</summary>
    ///<param name="source">The input <c>Composite</c>.</param>
    ///<typeparam name="T">The type of payload elements in the <c>Composite</c>.</typeparam>
    static member inline AsEnumerable (source: 'T Composite) =
        Enumerable.AsEnumerable(Seq.ofComp source)