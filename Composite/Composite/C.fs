namespace Composite

open System.Collections.Generic

module C =

    ///<summary>Instantiates a new composite variant of <c>Composite&lt;T&gt;</c>.</summary>
    ///<param name="components">An enumerable of components to wrap in a new composite.</param>
    ///<typeparam name="T">The type of payload objects in the composite.</typeparam>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>components</c> is null.</exception>
    let Composite (components: IEnumerable<'T Composite>) =
        if components |> isNull
        then nullArg "components"
        else Composite components

    ///<summary>Instantiates a new value variant of <c>Composite&lt;T&gt;</c>.</summary>
    ///<param name="value">The payload object.</param>
    ///<typeparam name="T">The type of the payload object.</typeparam>
    let Value (value: 'T) =
        Value value