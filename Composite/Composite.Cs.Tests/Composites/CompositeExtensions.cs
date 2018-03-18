using System;
using System.Collections.Generic;
using System.Linq;

using Composite;

namespace Composite.Cs.Tests.Composites
{
    internal static class CompositeExtensions
    {
        public static string AsString<T>(this Composite<T> source, Func<T, string> toStringFunc)
        {
            if(source is Composite<T>.Composite composite)
                return string.Format("[ {0} ]", string.Join(", ", composite.Item.Select(x => x.AsString(toStringFunc))));
            else
                return toStringFunc((source as Composite<T>.Value).Item);
        }
    }
}