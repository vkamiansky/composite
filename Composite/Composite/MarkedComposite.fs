namespace Composite

open System.Collections.Generic

[<RequireQualifiedAccess>]
module MarkedComposite =

    ///<summary>Initializes an empty marked composite.</summary>
    ///<param name="mark">The mark.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    let Empty<'TMark, 'TPayload> (mark: 'TMark) =
        MComp.empty<'TMark, 'TPayload> mark

    ///<summary>Instantiates a new composite variant of <c>MarkedComposite&lt;TMark,TPayload&gt;</c>.</summary>
    ///<param name="components">An enumerable of components to wrap in a new marked composite.</param>
    ///<typeparam name="TMark">The type of mark.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>components</c> is null.</exception>
    let Create (mark: 'TMark) (components: IEnumerable<Composite<'TMark, 'TPayload>>) =
        if components |> isNull
        then nullArg "components"
        else MarkedComposite {Mark = mark; Components = components}

    ///<summary>Instantiates a new value variant of <c>MarkedComposite&lt;TMark, TPayload&gt;</c>.</summary>
    ///<param name="value">The payload object.</param>
    ///<param name="mark">The mark.</param>
    ///<typeparam name="TMark">The type of the mark.</typeparam>
    ///<typeparam name="TPayload">The type of the payload object.</typeparam>
    let CreateValue (mark: 'TMark) (value: 'TPayload) =
        MarkedValue {Mark = mark; Value = value}
