namespace Composite

open System.Collections.Generic

module C =

    ///<summary>Instantiates a new <c>Composite</c> variant of <c>Composite<T></c>.
    ///<param name="innerForest">The inner forest to wrap in a <c>Composite</c>.</param>
    ///<typeparam name="T">The type of payload elements in the <c>Composite</c>.</typeparam>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>innerForest</c> is null.</exception>
    let Composite (innerForest: IEnumerable<'T Composite>) =
        if innerForest |> isNull
        then nullArg "innerForest"
        else Composite innerForest

    ///<summary>Instantiates a new <c>Value</c> variant of <c>Composite<T></c>.
    ///<param name="value">The payload object.</param>
    ///<typeparam name="T">The type of the payload object.</typeparam>
    let Value (value: 'T) =
        Value value