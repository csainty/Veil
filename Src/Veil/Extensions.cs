using System;
using System.Globalization;

namespace Veil
{
    internal static class Extensions
    {
        public static string FormatInvariant(this string format, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, format, args);
        }
    }
}