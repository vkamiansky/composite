using System;
using System.Collections.Generic;

namespace Composite.Cs.Tests {

    internal static class EnumerableHelper {
        public static IEnumerable<T> AsLimited<T>(this IEnumerable<T> source, int limit)
        {
            var i = 0;
            foreach(var element in source)
            {
                if (i++ < limit)
                  yield return element;
                else
                    throw new InvalidOperationException("You've attempted to walk through an infinite sequence.");
            }
        }
    }
}