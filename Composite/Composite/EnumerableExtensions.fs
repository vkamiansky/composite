namespace Composite

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.CompilerServices

[<Extension>]
type EnumerableExtensions () =

    [<Extension>]
    /// <summary>
    /// Splits the source enumerable into <c>numberPartitions</c> enumerables.
    /// </summary>
    /// <param name="source">
    /// The source enumerable.
    /// </param>
    /// <param name="numberPartitions">
    /// The number of partitions.
    /// </param>
    /// <returns>
    /// An array of partitions the <c>source</c> enumerable is distributed across.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <c>numberPartitions</c> is not positive.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when <c>source</c> is null.
    /// </exception>
    static member inline AsPartitioned (source: IEnumerable<'T>) (numberPartitions: int) =
        if source |> isNull then nullArg "source"
        if numberPartitions <= 0 then invalidArg "numberPartitions" "Number of partitions must not be 0 or negative."

        Seq.partition numberPartitions source |> Array.map Enumerable.AsEnumerable

    [<Extension>]
    /// <summary>
    /// Splits the <c>source</c> enumerable into pages of size at most <c>pageSize</c>.
    /// </summary>
    /// <param name="source">
    /// The source enumerable.
    /// </param>
    /// <param name="pageSize">
    /// The maximum size of each page.
    /// </param>
    /// <returns>
    /// An enumerable of pages of size at most <c>pageSize</c>.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <c>pageSize</c> is not positive.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when <c>source</c> is null.
    /// </exception>
    static member inline AsPaged (source: IEnumerable<'T>) (pageSize: int) =
        if source |> isNull then nullArg "source"
        if pageSize <= 0 then invalidArg "pageSize" "Page size must not be 0 or negative."

        Enumerable.AsEnumerable(Seq.chunkBySize pageSize source)

    [<Extension>]
    /// <summary>
    /// Splits the <c>source</c> enumerable into batches with total size of each multi-item batch no greater than <c>batchSize</c>.
    /// </summary>
    /// <param name="source">
    /// The source enumerable.
    /// </param>
    /// <param name="batchSize">
    /// The maximum total size of a multi-item batch.
    /// </param>
    /// <param name="getElementSize">
    /// A function that calculates the size of an element.
    /// </param>
    /// <returns>
    /// An enumerable of batches with multi-item batches of size no greater than <c>batchSize</c>.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <c>batchSize</c> is not positive.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when <c>source</c> is null.
    /// </exception>
    static member inline AsBatched (source: IEnumerable<'T>) (batchSize: int) (getElementSize: Func<'T, int>) =
        if source |> isNull then nullArg "source"
        if batchSize <= 0 then invalidArg "batchSize" "Batch size must not be 0 or negative."

        Enumerable.AsEnumerable(Seq.chunkByWeight batchSize getElementSize.Invoke source)

    [<Extension>]
    /// <summary>
    /// Selects arrays of values from <c>source</c>, transforms them, and returns the results as an output enumerable.
    /// </summary>
    /// <param name="source">
    /// The source enumerable.
    /// </param>
    /// <param name="scenario">
    /// An array of selection and transformation rules.
    /// </param>
    /// <typeparam name="TSource">
    /// The type of elements in the source enumerable.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of elements in the result enumerable.
    /// </typeparam>
    /// <returns>
    /// An enumerable of results produced from source elements by using the specified selection and transformation 
    /// scenario.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <c>scenario</c> has rules with null or empty arrays of check functions.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when <c>source</c> or <c>scenario</c> is null.
    /// </exception>
    static member inline Cata (source: IEnumerable<'TSource>) (scenario: CheckTransformRule<'TSource, 'TResult>[]) =
        if source |> isNull then nullArg "source"
        if scenario |> isNull then nullArg "scenario"

        let scn = scenario |> Array.map (fun p -> p.CheckFunctions 
                                                    |> function
                                                        | null | [||] -> invalidArg "scenario" "A check-transform rule must contain at least one check function."
                                                        | funcs -> funcs |> Array.map (fun x -> x.Invoke) , p.TransformFunction.Invoke)
        Seq.accumulateCollect scn source