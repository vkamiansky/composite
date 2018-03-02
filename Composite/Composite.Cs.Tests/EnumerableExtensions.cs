using System;
using System.Collections.Generic;

namespace Composite.Cs.Tests
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> AsLimited<T>(this IEnumerable<T> source, int limit)
        {
            var i = 0;
            foreach (var element in source)
                if (i++ < limit) yield return element;
                else throw new InvalidOperationException("You've attempted to iterate past the limit"
                                                         + "designated for the maximum number of elements" 
                                                         + "produced by this enumerable.");
        }
    }
}