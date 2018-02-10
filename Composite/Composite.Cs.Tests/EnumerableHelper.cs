using System;
using System.Collections.Generic;

namespace Composite.Cs.Tests {

    internal static class EnumerableHelper {
        public static IEnumerable<T> AsLimited<T>(this T[] source, int limit)
        {
            var i = 0;
            while(true)
            {
                if (i < limit)
                {
                    yield return source[i];
                    i++;
                } else {
                    throw new ArgumentOutOfRangeException("You've attempted to walk through an infinite sequence.");
                }
            }
        }
    }
}