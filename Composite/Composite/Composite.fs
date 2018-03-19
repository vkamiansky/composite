namespace Composite

    type 'T Composite =
        | Value of 'T
        | Composite of seq<Composite<'T>>