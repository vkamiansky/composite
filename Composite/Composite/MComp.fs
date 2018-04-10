namespace Composite

open FSharp.Core

[<RequireQualifiedAccess>]
module MComp =

    ///<summary>Initializes an empty marked composite.</summary>
    ///<param name="mark">The mark.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    let empty<'TMark, 'TPayload> (mark: 'TMark) =
        MarkedComposite {Mark = mark; Components = Seq.empty<Composite<'TMark, 'TPayload>>}

    ///<summary>Returns the sequence of components of the input marked composite.</summary>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    let components (source: Composite<'TMark, 'TPayload>) =
        match source with
        | MarkedComposite x -> x.Components
        | x -> seq { yield x }

    ///<summary>Builds a new marked composite based on the source in which the value is set at the given mark in the given container.</summary>
    ///<param name="containerMark">The mark of the container in which the value should be set.</param>
    ///<param name="valueMark">The mark to set the value at.</param>
    ///<param name="value">The value to set.</param>
    ///<param name="markComparer">The function used to determine if two marks match.</param>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    let rec setValue (containerMark: 'TMark) (valueMark: 'TMark) (value: 'TPayload) (markComparer: 'TMark -> 'TMark -> bool) (source: Composite<'TMark, 'TPayload>) =
        match source with
        | MarkedComposite x ->  if x.Mark |> markComparer containerMark then
                                    MarkedComposite {Mark = x.Mark; Components = seq{
                                        let mutable valueFound = false
                                        for c in x.Components do
                                            match c with
                                            | MarkedValue y -> if y.Mark |> markComparer valueMark then
                                                                    valueFound <- true
                                                                    yield MarkedValue {Mark = y.Mark; Value = value}
                                                               else yield c
                                            | c -> yield c
                                        if not valueFound then yield MarkedValue {Mark = valueMark; Value = value}
                                    }}
                                else MarkedComposite {Mark = x.Mark; Components = x.Components |> Seq.map (setValue containerMark valueMark value markComparer) }
        | x -> x

    ///<summary>Builds a new marked composite based on the source ensuring that a container will be present at the given mark in the given container.</summary>
    ///<param name="outerContainerMark">The mark of the container in which the container should be found.</param>
    ///<param name="innerContainerMark">The mark to the container of which the presence is to be ensured.</param>
    ///<param name="markComparer">The function used to determine if two marks match.</param>
    ///<param name="source">The input composite.</param>
    ///<typeparam name="TMark">The type of marks in the composite.</typeparam>
    ///<typeparam name="TPayload">The type of payload objects in the composite.</typeparam>
    let rec ensureHasContainer (outerContainerMark: 'TMark) (innerContainerMark: 'TMark) (markComparer: 'TMark -> 'TMark -> bool) (source: Composite<'TMark, 'TPayload>) =
        match source with
        | MarkedComposite x ->  if x.Mark |> markComparer outerContainerMark then
                                    MarkedComposite {Mark = x.Mark; Components = seq{
                                        let mutable containerFound = false
                                        for c in x.Components do
                                            match c with
                                            | MarkedComposite y -> if y.Mark |> markComparer innerContainerMark then
                                                                    containerFound <- true
                                                                    yield c
                                                                   else yield c
                                            | c -> yield c
                                        if not containerFound then yield MarkedComposite {Mark = innerContainerMark; Components = Seq.empty}
                                    }}
                                else MarkedComposite {Mark = x.Mark; Components = x.Components |> Seq.map (ensureHasContainer outerContainerMark innerContainerMark markComparer) }
        | x -> x