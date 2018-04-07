using System;
using System.Collections.Generic;
using System.Linq;

namespace Composite.Cs.Tests.Infrastructure
{
    internal static class EnumerableExtensions
    {
        private const string  _AsLimitedWalkPastLimitErrorMessage = "You've attempted to iterate past the limit"
                                                                    + "designated for the maximum number of elements" 
                                                                    + "produced by this enumerable.";
        public static IEnumerable<T> AllowTake<T>(this IEnumerable<T> source, int limit)
        {            
            if(limit < 1)
            {
                throw new InvalidOperationException(_AsLimitedWalkPastLimitErrorMessage);
            }
            var i = 0;
            foreach (var element in source)
                if (i++ < limit) yield return element;
                else throw new InvalidOperationException(_AsLimitedWalkPastLimitErrorMessage);
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