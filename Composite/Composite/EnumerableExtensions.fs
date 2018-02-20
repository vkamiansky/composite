namespace Composite

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.CompilerServices

open Transforms

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
    static member inline AsPartitioned (source: IEnumerable<'T>) (numberPartitions: int) =
        toPartitioned numberPartitions source |> Array.map Enumerable.AsEnumerable

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
    static member inline AsPaged (source: IEnumerable<'T>) (pageSize: int) =
        Enumerable.AsEnumerable(toPaged pageSize source)

    [<Extension>]
    /// <summary>
    /// Splits the <c>source</c> enumerable into batches with total size of
    /// each multi-item batch no greater than <c>batchSize</c>.
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
    /// An enumerable of batches with multi-item batches of size no greater than
    /// <c>batchSize</c>.
    /// </returns>
    static member inline AsBatched (source: IEnumerable<'T>) (batchSize: int) (getElementSize: Func<'T, int>) =
        Enumerable.AsEnumerable(toBatched batchSize getElementSize.Invoke source)

    
    [<Extension>]
    static member inline Cata (source: IEnumerable<'a>) (scenario: CheckTransformPair<'a, 'b>[]) =
        let scn = scenario |> Array.map (fun p -> p.CheckFunctions |> Array.map (fun x -> x.Invoke) , p.TransformFunction.Invoke)
        cata scn source