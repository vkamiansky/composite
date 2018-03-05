using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<T> WithSideEffect<T>(this IEnumerable<T> source, Action<T> sideEffect)
        {
            return source.Select(x =>
            {
                sideEffect(x);
                return x;
            });
        }
    }
}