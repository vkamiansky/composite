namespace Composite

open System
open System.Collections.Generic

    ///<summary>Represents a set of functions used to find objects, accumulate them in an array, and then transform the accumulated objects into an output enumerable.</summary>
    type AccumulateTransformRule<'TSource, 'TResult> =

        ///<summary>An array of predicates used to find objects to be put in the accumulator cells with the respective indices.</summary>
        val FindPredicates: Func<'TSource, bool>[]

        ///<summary>A function used to transform the array of accumulated objects into an enumerable result.</summary>
        val TransformFunction: Func<'TSource[], IEnumerable<'TResult>>

        ///<summary>Creates a new AccumulateTransformRule.</summary>
        ///<param name="findPredicates">An array of predicates used to find objects to be put in the accumulator cells with the respective indices.</param>
        ///<param name="transformFunction">A function used to transform the array of accumulated objects into an enumerable result.</param>
        ///<typeparam name="TSource">The type of objects in the source enumerable.</typeparam>
        ///<typeparam name="TResult">The type of objects in the result enumerable.</typeparam>
        new (findPredicates, transformFunction) =
             { FindPredicates = findPredicates
               TransformFunction = transformFunction }
