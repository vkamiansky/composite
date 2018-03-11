namespace Composite

    type 'a Composite =
        | Value of 'a
        | Composite of seq<Composite<'a>>