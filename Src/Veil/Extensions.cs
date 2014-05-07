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

        private static bool IsDictionary(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        public static bool HasEnumerableInterface(this Type t)
        {
            return IsEnumerableType(t) || t.GetInterfaces().Any(IsEnumerableType);
        }

        public static Type GetEnumerableInterface(this Type t)
        {
            return IsEnumerableType(t) ? t : t.GetInterfaces().First(IsEnumerableType);
        }

        public static Type GetDictionaryTypeWithKey<TKey>(this Type t)
        {
            Type dictionaryType;
            if (IsDictionary(t)) dictionaryType = t;
            else dictionaryType = t.GetInterfaces().FirstOrDefault(IsDictionary);

            if (dictionaryType == null) return null;
            if (dictionaryType.GetGenericArguments()[0] != typeof(string)) return null;
            return dictionaryType;
        }
    }
}