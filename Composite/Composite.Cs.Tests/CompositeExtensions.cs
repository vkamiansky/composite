using System;
using System.Collections.Generic;
using System.Linq;

using Composite;

namespace Composite.Cs.Tests
{
    internal static class CompositeExtensions
    {
        public static string AsString<T>(this DataTypes.Composite<T> source, Func<T, string> stringifyFunc)
        {
            if(source is DataTypes.Composite<T>.Composite composite)
                return string.Format("[ {0} ]", string.Join(", ", composite.Item.Select(x => x.AsString(stringifyFunc))));
            else
                return stringifyFunc((source as DataTypes.Composite<T>.Value).Item);
        }
    }
}