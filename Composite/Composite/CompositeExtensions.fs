namespace Composite

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.CompilerServices

open FSharp.Core

[<Extension>]
type CompositeExtensions () =

    [<Extension>]
    ///<summary>Builds a new composite based on the source in which the values for which the given predicate returns <c>true</c> are substituted by composites containing values produced by using the given function.</summary>
    ///<param name="source">The input composite.</param>
    ///<param name="predicate">A function to test whether each value from the input composite should be transformed into a composite.</param>
    ///<param name="mapping">An object-to-sequence function to transform values from the input composite into composites.</param>
    ///<typeparam name="T">The type of payload objects in the composite.</typeparam>
    static member inline Fork (source: 'T Composite) (predicate: Func<'T, bool>) (mapping: Func<'T, IEnumerable<'T>>) =
        source |> Comp.fork predicate.Invoke mapping.Invoke

    [<Extension>]
    ///<summary>Builds a new composite based on the source in which the values are the results of applying the given function to each of the values in the input composite.</summary>
    ///<param name="source">The input composite.</param>
    ///<param name="mapping">A function to transform values from the input composite.</param>
    ///<typeparam name="TIn">The type of payload objects in the input composite.</typeparam>
    ///<typeparam name="TOut">The type of payload objects in the output composite.</typeparam>
    static member inline Select (source: 'TIn Composite) (mapping: Func<'TIn, 'TOut>) =
        source |> Comp.map mapping.Invoke

    [<Extension>]
    ///<summary>Returns the enumerable of components of the input composite.</summary>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="T">The type of payload objects in the composite.</typeparam>
    static member inline ToComponents (source: 'T Composite) =
        source |> Comp.components |> Enumerable.AsEnumerable

    [<Extension>]
    ///<summary>Views the given composite as an enumerable.</summary>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="T">The type of payload objects in the composite.</typeparam>
    static member inline AsEnumerable (source: 'T Composite) =
        source |> Seq.ofComp |> Enumerable.AsEnumerable