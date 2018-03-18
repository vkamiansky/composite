namespace Composite

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.CompilerServices

[<Extension>]
type EnumerableExtensions () =

    [<Extension>]
    ///<summary>Splits the source enumerable into <c>numberPartitions</c> enumerables.</summary>
    ///<param name="source">The source enumerable.</param>
    ///<param name="numberPartitions">The number of partitions.</param>
    ///<returns>An array of partitions the <c>source</c> enumerable is distributed across.</returns>
    ///<exception cref="System.ArgumentException">Thrown when <c>numberPartitions</c> is not positive.</exception>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>source</c> is null.</exception>
    static member inline AsPartitioned (source: IEnumerable<'T>) (numberPartitions: int) =
        if source |> isNull then nullArg "source"
        if numberPartitions <= 0 then invalidArg "numberPartitions" "Number of partitions must not be 0 or negative."

        source |> Seq.partition numberPartitions |> Array.map Enumerable.AsEnumerable

    [<Extension>]
    ///<summary>Splits the <c>source</c> enumerable into pages of size at most <c>pageSize</c>.</summary>
    ///<param name="source">The source enumerable.</param>
    ///<param name="pageSize">The maximum size of each page.</param>
    ///<returns>An enumerable of pages of size at most <c>pageSize</c>.</returns>
    ///<exception cref="System.ArgumentException">Thrown when <c>pageSize</c> is not positive.</exception>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>source</c> is null.</exception>
    static member inline AsPaged (source: IEnumerable<'T>) (pageSize: int) =
        if source |> isNull then nullArg "source"
        if pageSize <= 0 then invalidArg "pageSize" "Page size must not be 0 or negative."

        source |> Seq.chunkBySize pageSize |> Enumerable.AsEnumerable

    [<Extension>]
    ///<summary>Divides the input sequence into batches of as many elements as possible their total size not exceeding <c>batchSize</c> but not less than one element per batch.</summary>
    ///<param name="source">The source enumerable.</param>
    ///<param name="batchSize">The maximum total size of a multi-item batch.</param>
    ///<param name="getElementSize">A function that calculates the size of an element.</param>
    ///<returns>An enumerable of batches with multi-item batches of size no greater than <c>batchSize</c>.</returns>
    ///<exception cref="System.ArgumentException">Thrown when <c>batchSize</c> is not positive.</exception>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>source</c> is null.</exception>
    static member inline AsBatched (source: IEnumerable<'T>) (batchSize: int) (getElementSize: Func<'T, int>) =
        if source |> isNull then nullArg "source"
        if batchSize <= 0 then invalidArg "batchSize" "Batch size must not be 0 or negative."

        source |> Seq.chunkByWeight batchSize getElementSize.Invoke |> Enumerable.AsEnumerable

    [<Extension>]
    ///<summary>Finds elements in the input sequence to accumulate them in arrays, applies the respective transform functions to populated arrays, and concatenates the results.</summary>
    ///<param name="source">The source enumerable.</param>
    ///<param name="rules">An array of accumulation and transformation rules.</param>
    ///<typeparam name="TSource">The type of elements in the source enumerable.</typeparam>
    ///<typeparam name="TResult">The type of elements in the result enumerable.</typeparam>
    ///<returns>An enumerable of results produced from source elements by using the specified accumulation and transformation rules.</returns>
    ///<exception cref="System.ArgumentException">Thrown when some of the <c>rules</c> have null or empty arrays of check functions.</exception>
    ///<exception cref="System.ArgumentNullException">Thrown when <c>source</c> or <c>rules</c> is null.</exception>
    static member inline AccumulateSelectMany (source: IEnumerable<'TSource>) (rules: AccumulateTransformRule<'TSource, 'TResult>[]) =
        if source |> isNull then nullArg "source"
        if rules |> isNull then nullArg "rules"

        let rulesParam = rules |> Array.map (fun p -> p.FindPredicates 
                                                    |> function
                                                        | null | [||] -> invalidArg "rules" "An accumulate-transform rule must contain at least one find predicate."
                                                        | funcs -> funcs |> Array.map (fun x -> x.Invoke) , p.TransformFunction.Invoke)
        source |> Seq.accumulateCollect rulesParam