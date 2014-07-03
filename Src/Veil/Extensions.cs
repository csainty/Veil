using System;
using System.Collections;
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

        private static bool IsDictionaryType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        private static bool IsCollectionType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        private static bool IsNonGenericCollectionType(Type t)
        {
            return typeof(ICollection).IsAssignableFrom(t);
        }

        public static bool HasEnumerableInterface(this Type t)
        {
            return IsEnumerableType(t) || t.GetInterfaces().Any(IsEnumerableType);
        }

        public static bool HasCollectionInterface(this Type t)
        {
            return IsNonGenericCollectionType(t) || IsCollectionType(t) || t.GetInterfaces().Any(IsCollectionType);
        }

        public static Type GetEnumerableInterface(this Type t)
        {
            return IsEnumerableType(t) ? t : t.GetInterfaces().First(IsEnumerableType);
        }

        public static Type GetCollectionInterface(this Type t)
        {
            return IsNonGenericCollectionType(t) ? typeof(ICollection) : (IsCollectionType(t) ? t : t.GetInterfaces().First(IsCollectionType));
        }

        public static Type GetDictionaryTypeWithKey<TKey>(this Type t)
        {
            Type dictionaryType;
            if (IsDictionaryType(t)) dictionaryType = t;
            else dictionaryType = t.GetInterfaces().FirstOrDefault(IsDictionaryType);

            if (dictionaryType == null) return null;
            if (dictionaryType.GetGenericArguments()[0] != typeof(string)) return null;
            return dictionaryType;
        }
    }
}