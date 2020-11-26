using System;
using System.Collections.Generic;
using System.Text;

namespace SparkPost.Tests
{
    internal static class CastAsExtensions
    {
        internal static T CastAs<T>(this object dict)
        {
            return (T)dict;
        }
    }
}
