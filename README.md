 # composite

[![Build status](https://ci.appveyor.com/api/projects/status/51ll2t40ae4mhtaf/branch/master?svg=true)](https://ci.appveyor.com/project/vkamiansky/composite)

[A *sequence* is a logical series of elements all of one type](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/sequences). The definition of a sequence has no stipulation of how its elements are stored, or linked, or whether they are stored at all. In C# sequences are called enumerable collections, they implement `IEnumerable<T>`. In F# they are represented by the `seq<T>` type.

The main purpose of *Project Composite* is to provide extensions or functions that allow building sequence-based data processing solutions in C#/F# with more convenience and practical sense.

It also introduces `Composite<T>`, a brand of sequence with a native support for nesting. Each node of a `Composite<T>` is either a `Value` containing a payload object or a `Composite` comprising a sequence of child nodes.

The functions are listed below grouped by their C#-friendly representations followed by their F#-friendly signatures.

## Sequence Extensions

### IEnumerable&lt;T&gt;[] EnumerableExtensions.AsPartitioned&lt;T&gt;(IEnumerable&lt;T&gt; source, int numberPartitions)
#### F#: Seq.partition: (numParts: int) -> (source: seq&lt;'T&gt;) -> seq&lt;'T&gt; []

Splits the source sequence into an array of sequences.

The function uses deferred execution. The resulting sequences will not be generated until they are enumerated as shown in the diagram below.

![AsPartitioned diagram](https://raw.github.com/wiki/vkamiansky/composite/diagrams/AsPartitioned.svg?sanitize=true)

### IEnumerable&lt;T[]&gt; EnumerableExtensions.AsPaged&lt;T&gt;(IEnumerable&lt;T&gt; source, int pageSize)
#### F# (available in standard _FSharp.Core_ since version 4.4.0.0): Seq.chunkBySize: (chunkSize: int) -> (source: seq&lt;'T&gt;) -> seq&lt;'T[]&gt;

Divides the input sequence into a sequence of pages each containing at most the given number of elements.

The function uses deferred execution. The resulting sequence will not be generated until it is enumerated as shown in the diagram below.

![AsPaged diagram](https://raw.github.com/wiki/vkamiansky/composite/diagrams/AsPaged.svg?sanitize=true)

### IEnumerable&lt;T[]&gt; EnumerableExtensions.AsBatched&lt;T&gt;(IEnumerable&lt;T&gt; source, int batchSize, Func&lt;T, int&gt; getElementSize)
#### F#: Seq.chunkByWeight: (chunkWeight: int) -> (weigh: 'T -> int) -> (source: seq&lt;'T&gt;) -> seq&lt;'T[]&gt;

Divides the input sequence into a sequence of batches of either one element or as many elements as possible if their total size does not exceed the given number. The size of each element is obtained through the use of the given function.

The function uses deferred execution. The resulting sequence will not be generated until it is enumerated as shown in the diagram below.

![AsBatched diagram](https://raw.github.com/wiki/vkamiansky/composite/diagrams/AsBatched.svg?sanitize=true)

### IEnumerable&lt;TResult&gt; EnumerableExtensions.AccumulateSelectMany&lt;TSource&gt;(IEnumerable&lt;TSource&gt; source, AccumulateTransformRule&lt;TSource, TResult&gt;[] rules)
#### F#: Seq.accumulateCollect (rules: (('TIn -> bool)[] * ('TIn[] -> seq&lt;'TOut&gt;))[]) -> (source: seq&lt;'TIn&gt;) -> seq&lt;'TOut&gt;

Finds elements in the input sequence to accumulate them in arrays, applies the respective transform functions to populated arrays, and concatenates the results. The accumulation and transformation of objects found in the source sequence is performed according to the given rules.

`AccumulateTransformRule`has the following constructor signature:
```c#
public AccumulateTransformRule(Func<TSource, bool>[] findPredicates, Func<TSource[], IEnumerable<TResult>> transformFunction)
```

The `findPredicates` are used to find objects to be put in the accumulator cells with the respective indices.

The `transformFunction` is used to transform the array of accumulated objects into an enumerable result.

The function uses deferred execution. The resulting sequence will not be generated until it is enumerated as shown in the diagram below if we assume the `rules` are defined as in the following listing.

```c#
var rules = new[] { new AccumulateTransformRule<int, int>(
                        new Func<int, bool>[] { 
                            x => x > 2, 
                            x => x > 3 },
                        x => new[] { x[0] + x[1] + 1, 2 * x[0] + x[1] / 2 } ),
                    new AccumulateTransformRule<int, int>(
                        new Func<int, bool>[] { 
                            x => x > 1, 
                            x => x > 2 },
                        x => new[] { 2 * x[0] + x[1], 3 * x[0] + x[1] } )
                   };
```
![AccumulateSelectMany diagram](https://raw.github.com/wiki/vkamiansky/composite/diagrams/AccumulateSelectMany.svg?sanitize=true)

## Composite Extensions

### Composite&lt;T&gt; CompositeExtensions.Fork&lt;T&gt;(Composite&lt;T&gt; source, Func&lt;T, bool&gt; predicate, Func&lt;T, T[]&gt; mapping)
#### F#: Comp.fork (predicate: 'T -> bool) -> (mapping: 'T -> seq&lt;'T&gt;) -> (source: 'T Composite) -> 'T Composite

Builds a new sequence composite based on the source in which the values containing objects for which the given predicate returns `true` are substituted for sequence composites wrapping the sequences produced from them through the use of the mapping function.

The function uses deferred execution. The resulting sequence composite will not be generated until it is enumerated as shown in the diagram below.

![Fork diagram](https://raw.github.com/wiki/vkamiansky/composite/diagrams/Fork.svg?sanitize=true)

### Composite&lt;TOut&gt; CompositeExtensions.Select&lt;TIn,TOut&gt;(Composite&lt;TIn&gt; source, Func&lt;TIn, TOut&gt; mapping)
#### F#: Comp.map (mapping: 'TIn -> 'TOut) -> (source: 'TIn Composite) -> 'TOut Composite

Builds a new sequence composite based on the source in which the values contain objects substituted through the use of the given function.

The function uses deferred execution. The resulting sequence composite will not be generated until it is enumerated as shown in the diagram below.

![Select diagram](https://raw.github.com/wiki/vkamiansky/composite/diagrams/Select.svg?sanitize=true)

### IEnumerable&lt;Composite&lt;T&gt;&gt; CompositeExtensions.ToComponents&lt;T&gt;(Composite&lt;T&gt; source)
#### F#: Comp.components (source: 'T Composite) -> seq&lt;'T Composite&gt;

Returns the sequence of components of the input sequence composite.

The function uses deferred execution. The resulting sequence will not be generated until it is enumerated as shown in the diagram below.

![ToComponents diagram](https://raw.github.com/wiki/vkamiansky/composite/diagrams/ToComponents.svg?sanitize=true)

### IEnumerable&lt;T&gt; CompositeExtensions.AsEnumerable&lt;T&gt;(Composite&lt;T&gt; source)
#### F#: Seq.ofComp (source: 'T Composite) -> seq&lt;'T&gt;

Views the given sequence composite as a sequence.

The function uses deferred execution. The resulting sequence will not be generated until it is enumerated as shown in the diagram below.

![AsEnumerable diagram](https://raw.github.com/wiki/vkamiansky/composite/diagrams/AsEnumerable.svg?sanitize=true)
