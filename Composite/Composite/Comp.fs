namespace Composite

open FSharp.Core

[<RequireQualifiedAccess>]
module Comp =

    ///<summary>Returns the sequence of components of the input composite.</summary>
    ///<param name="source">The input composite.</param>
    let components (source: 'T Composite) =
        match source with
        | Composite x -> x
        | x -> seq { yield x }

    ///<summary>Builds a new composite based on the source in which the values for which the given predicate returns <c>true</c> are substituted for composites containing the objects produced by using the given function as their value components.</summary>
    ///<param name="predicate">A function to test whether each value in the input composite should be transformed into a composite.</param>
    ///<param name="mapping">A function to transform values from the input composite.</param>
    ///<param name="source">The input composite.</param>
    let rec fork (predicate: 'T -> bool) (mapping: 'T -> seq<'T>) (source: 'T Composite) =
        match source with
        | Composite x -> x |> Seq.map (fork predicate mapping) |> Composite
        | Value x -> if predicate x
                     then mapping x |> Seq.map Value |> Composite
                     else source

    ///<summary>Builds a new composite based on the <c>source</c> in which the values are the results of applying the given function to each of the values in the input composite.</summary>
    ///<param name="mapping">A function to transform values from the input composite.</param>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="TIn">The type of payload objects in the input composite.</typeparam>
    ///<typeparam name="TOut">The type of payload objects in the output composite.</typeparam>
    let rec map (mapping: 'TIn -> 'TOut) (source: 'TIn Composite) =
        match source with
        | Composite x -> x |> Seq.map (map mapping) |> Composite
        | Value x -> x |> mapping |> Value