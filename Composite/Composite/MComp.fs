namespace Composite

open FSharp.Core

[<RequireQualifiedAccess>]
module MComp =

    ///<summary>Returns the sequence of components of the input marked composite.</summary>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    let components (source: Composite<'TMark, 'TPayload>) =
        match source with
        | MarkedComposite x -> x.Components
        | x -> seq { yield x }