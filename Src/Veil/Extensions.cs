using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Veil
{
    internal static class Extensions
    {
        public static string FormatInvariant(this string format, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, format, args);
        }

        private static bool IsEnumerableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool HasEnumerableInterface(this Type t)
        {
            return IsEnumerableType(t) || t.GetInterfaces().Any(IsEnumerableType);
        }

        public static Type GetEnumerableInterface(this Type t)
        {
            return IsEnumerableType(t) ? t : t.GetInterfaces().First(IsEnumerableType);
        }
    }
}