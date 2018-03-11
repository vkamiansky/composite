namespace Composite

open System
open System.Collections.Generic

    ///<summary>Represents a set of functions used to transform an array of objects selected from an enumerable of type <c>TSource</c> to an enumerable of type <c>TResult</c>.</summary>
    type CheckTransformRule<'TSource, 'TResult> =

        ///<summary>An array of functions each used to check whether an object can be selected to become the element with the same index in the array of transform parameters.</summary>
        val CheckFunctions: Func<'TSource, bool>[]

        ///<summary>A function used to transform the array of selected parameters to an enumerable result.</summary>
        val TransformFunction: Func<'TSource[], IEnumerable<'TResult>>

        ///<summary>Creates a new CheckTransformRule.</summary>
        ///<param name="checkFunctions">An array of functions each used to check whether an element of the source enumerable can be selected to become the element with the same index in the array of transform parameters.</param>
        ///<param name="transformFunction">A function used to transform the array of selected parameters to an enumerable result.</param>
        ///<typeparam name="TSource">The type of the source object.</typeparam>
        ///<typeparam name="TResult">The type of elements in the enumerable result.</typeparam>
        new (checkFunctions, transformFunction) =
             { CheckFunctions = checkFunctions
               TransformFunction = transformFunction }
