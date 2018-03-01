namespace Composite

open System
open System.Collections.Generic

    /// <summary>
    /// Represents a set of functions used to unfold an object of type <c>T</c> to an enumerable of the same type.
    /// </summary>
    type CheckUnfoldRule<'T> =

        /// <summary>
        /// A function determining whether the rule can be applied to the given object of type <c>T</c>.
        /// </summary>
        val CheckFunction: Func<'T, bool>

        /// <summary>
        /// A function used to unfold the given object of type <c>T</c> into an enumerable result.
        /// </summary>
        val UnfoldFunction: Func<'T, IEnumerable<'T>>

        /// <summary>
        /// Creates a new CheckUnfoldRule.
        /// </summary>
        /// <param name="checkFunction">
        /// A function determining whether the rule can be applied to the given object of type <c>T</c>.
        /// </param>
        /// <param name="unfoldFunction">
        /// A function used to unfold the given object of type <c>T</c> into an enumerable result.
        /// </param>
        /// <typeparam name="T">
        /// The type of the unfolding object.
        /// </typeparam>
        new (checkFunction, unfoldFunction) =
             { CheckFunction = checkFunction
               UnfoldFunction = unfoldFunction }        
