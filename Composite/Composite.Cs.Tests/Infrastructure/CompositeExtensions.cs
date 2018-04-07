using System;
using System.Collections.Generic;
using System.Linq;

using Composite;

namespace Composite.Cs.Tests.Infrastructure
{
    internal static class CompositeExtensions
    {
        public static string ToStringShort<T>(this Composite<T> source)
        {
            if(source is Composite<T>.Composite composite)
            {
                var innerString = string.Join(", ", composite.Item.Select(x => x.ToStringShort()));
                return string.Format(string.IsNullOrEmpty(innerString) ? "[ ]" : "[ {0} ]", innerString);
            }
            else
                return (source as Composite<T>.Value).Item.ToString();
        }
    }
}