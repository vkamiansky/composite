namespace Composite

open System
open System.Collections.Generic

    /// <summary>
    /// Represents a set of functions used to unfold an object
    /// of type <c>T</c> to an enumerable of the same type.
    /// </summary>
    type CheckUnfoldRule<'T> private (checkFunction, unfoldFunction, foo: unit) =

        /// <summary>
        /// A function determining whether the rule can be applied
        /// to a given object of type <s>T</s>.
        /// </summary>
        member this.CheckFunction: Func<'T, bool> = checkFunction

        /// <summary>
        /// A function used to unfold a given object of type
        /// <s>T</s> into an enumerable result.
        /// </summary>
        member this.UnfoldFunction: Func<'T, IEnumerable<'T>> = unfoldFunction

        /// <summary>
        /// Creates a new CheckUnfoldRule
        /// </summary>
        /// <param name="checkFunction">
        /// A function determining whether the rule can be applied
        /// to a given object of type <s>T</s>.
        /// </param>
        /// <param name="unfoldFunction">
        /// A function used to unfold a given object of type
        /// <s>T</s> into an enumerable result.
        /// </param>
        /// <typeparam name="T">
        /// The type of the unfolding object.
        /// </typeparam>
        new (checkFunction, unfoldFunction) =
             CheckUnfoldRule (checkFunction, unfoldFunction, ())        
