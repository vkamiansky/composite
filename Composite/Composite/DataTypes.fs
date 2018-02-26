namespace Composite

module DataTypes =

    type 'a Composite =
        | Value of 'a
        | Composite of seq<Composite<'a>>