namespace Composite

open FSharp.Core

[<RequireQualifiedAccess>]
module MarkedComposite =

    ///<summary>Initializes an empty marked composite.</summary>
    ///<param name="mark">The mark.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    let Empty<'TMark, 'TPayload> (mark: 'TMark) =
        MComp.empty<'TMark, 'TPayload> mark
