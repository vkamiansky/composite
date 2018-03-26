# composite

[![Build status](https://ci.appveyor.com/api/projects/status/51ll2t40ae4mhtaf/branch/master?svg=true)](https://ci.appveyor.com/project/vkamiansky/composite)

[A *sequence* is a logical series of elements all of one type](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/sequences). The definition of a sequence has no stipulation of how its elements are stored, or linked, or whether they are stored at all. In C# sequences are called enumerable collections, they implement `IEnumerable<T>`. In F# they are represented by the `seq<T>` type.

The main purpose of *Project Composite* is to provide extensions or functions that allow building sequence-based data processing solutions in C#/F# with more convenience and practical sense.

It also introduces `Composite<T>`, a brand of sequence with a native support for nesting. Each node of a `Composite<T>` is either a `Value` containing a payload object or a `Composite` comprising a sequence of child nodes.

The functions hereby introduced all have their C#- and F#- friendly interfaces/aliases.
