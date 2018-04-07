namespace Composite

    type MarkValuePair<'TMark, 'TValue> = {Mark: 'TMark; Value: 'TValue}

    type MarkComponentsPair<'TMark, 'TComponent> = {Mark: 'TMark; Components: seq<'TComponent>}

    type 'T Composite =
        | Value of 'T
        | Composite of seq<Composite<'T>>

    type Composite<'TMark,'TPayload> =
        | MarkedValue of MarkValuePair<'TMark, 'TPayload>
        | MarkedComposite of MarkComponentsPair<'TMark, Composite<'TMark,'TPayload>>