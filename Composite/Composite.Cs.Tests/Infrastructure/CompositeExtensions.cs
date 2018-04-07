using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Core;

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

        public static string ToStringShort<TMark, TPayload>(this Composite<TMark, TPayload> source)
        {
            if(source is Composite<TMark, TPayload>.MarkedComposite composite)
            {
                var innerString = string.Join(", ", composite.Item.Components.Select(x => x.ToStringShort()));
                var markString = composite.Item.Mark.ToString();
                return string.Format(string.IsNullOrEmpty(markString) ? string.Empty : "( {0} )" , markString) +
                    string.Format(string.IsNullOrEmpty(innerString) ? "[ ]" : "[ {0} ]", innerString);
            }
            else if(source is Composite<TMark, TPayload>.MarkedValue value)
            {
                var markString = value.Item.Mark.ToString();
                return string.Format(string.IsNullOrEmpty(markString) ? string.Empty : "( {0} )" , markString) +
                    value.Item.Value.ToString();
            }
            else return string.Empty;
        }
    }
}