namespace Composite

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.CompilerServices

open FSharp.Core

[<Extension>]
type CompositeExtensions () =

    [<Extension>]
    ///<summary>Builds a new composite based on the source in which the values for which the given predicate returns <c>true</c> are substituted for composites containing the objects produced by using the given function as their value components.</summary>
    ///<param name="source">The input composite.</param>
    ///<param name="predicate">A function to test whether each value in the input composite should be transformed into a composite.</param>
    ///<param name="mapping">A function to transform objects from the input composite.</param>
    ///<typeparam name="T">The type of payload objects in the composite.</typeparam>
    static member inline Fork (source: 'T Composite) (predicate: Func<'T, bool>) (mapping: Func<'T, IEnumerable<'T>>) =
        if predicate |> isNull then nullArg "predicate"
        if mapping |> isNull then nullArg "mapping"

        source |> Comp.fork predicate.Invoke mapping.Invoke

    [<Extension>]
    ///<summary>Builds a new composite based on the source in which the values are the results of applying the given function to each of the values in the input composite.</summary>
    ///<param name="source">The input composite.</param>
    ///<param name="mapping">A function to transform values from the input composite.</param>
    ///<typeparam name="TIn">The type of payload objects in the input composite.</typeparam>
    ///<typeparam name="TOut">The type of payload objects in the output composite.</typeparam>
    static member inline Select (source: 'TIn Composite) (mapping: Func<'TIn, 'TOut>) =
        if mapping |> isNull then nullArg "mapping"

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

    [<Extension>]
    ///<summary>Returns the enumerable of components of the input marked composite.</summary>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    static member inline ToComponents (source: Composite<'TMark, 'TPayload>) =
        source |> MComp.components |> Enumerable.AsEnumerable

    [<Extension>]
    ///<summary>Views the given marked composite as an enumerable.</summary>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    static member inline AsEnumerable (source: Composite<'TMark, 'TPayload>) =
        source |> Seq.ofMComp |> Enumerable.AsEnumerable

    [<Extension>]
    ///<summary>Builds a new marked composite based on the source in which the value is set at the given mark in the given container.</summary>
    ///<param name="source">The input composite.</param>
    ///<param name="containerMark">The mark of the container in which the value should be set.</param>
    ///<param name="valueMark">The mark to set the value at.</param>
    ///<param name="value">The value to set.</param>
    ///<param name="markComparer">The function used to determine if two marks match.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    static member inline SetValue (source: Composite<'TMark, 'TPayload>) (containerMark: 'TMark) (valueMark: 'TMark) (value: 'TPayload) (markComparer: Func<'TMark,'TMark,bool>) =
        source |> MComp.setValue containerMark valueMark value (fun x1 x2-> markComparer.Invoke(x1, x2))

    [<Extension>]
    ///<summary>Builds a new marked composite based on the source ensuring that a container will be present at the given mark in the given container.</summary>
    ///<param name="source">The input composite.</param>
    ///<param name="outerContainerMark">The mark of the container in which the container should be found.</param>
    ///<param name="innerContainerMark">The mark to the container of which the presence is to be ensured.</param>
    ///<param name="markComparer">The function used to determine if two marks match.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    static member inline EnsureHasContainer (source: Composite<'TMark, 'TPayload>) (outerContainerMark: 'TMark) (innerContainerMark: 'TMark) (markComparer: Func<'TMark,'TMark,bool>) =
        source |> MComp.ensureHasContainer outerContainerMark innerContainerMark (fun x1 x2-> markComparer.Invoke(x1, x2))