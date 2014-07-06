using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace Veil
{
    internal static class Helpers
    {
        public static void HtmlEncode(TextWriter writer, string value)
        {
            if (value == null || value.Length == 0) return;
            var startIndex = 0;
            var currentIndex = 0;
            var valueLength = value.Length;
            char currentChar;

            if (valueLength < 50)
            {
                // For short string, we can just pump each char directly to the writer
                for (; currentIndex < valueLength; ++currentIndex)
                {
                    currentChar = value[currentIndex];
                    switch (currentChar)
                    {
                        case '&': writer.Write("&amp;"); break;
                        case '<': writer.Write("&lt;"); break;
                        case '>': writer.Write("&gt;"); break;
                        case '"': writer.Write("&quot;"); break;
                        case '\'': writer.Write("&#39;"); break;
                        default: writer.Write(currentChar); break;
                    }
                }
            }
            else
            {
                // For longer strings, the number of Write calls becomes prohibitive, so sacrifice a call to ToCharArray to allos us to buffer the Write calls
                char[] chars = null;
                for (; currentIndex < valueLength; ++currentIndex)
                {
                    currentChar = value[currentIndex];
                    switch (currentChar)
                    {
                        case '&':
                        case '<':
                        case '>':
                        case '"':
                        case '\'':
                            if (chars == null) chars = value.ToCharArray();
                            if (currentIndex != startIndex) writer.Write(chars, startIndex, currentIndex - startIndex);
                            startIndex = currentIndex + 1;

                            switch (currentChar)
                            {
                                case '&': writer.Write("&amp;"); break;
                                case '<': writer.Write("&lt;"); break;
                                case '>': writer.Write("&gt;"); break;
                                case '"': writer.Write("&quot;"); break;
                                case '\'': writer.Write("&#39;"); break;
                            }
                            break;
                    }
                }

                if (startIndex == 0) writer.Write(value);
                else if (currentIndex != startIndex) writer.Write(chars, startIndex, currentIndex - startIndex);
            }
        }

        public static void HtmlEncodeLateBound(TextWriter writer, object value)
        {
            if (value is string)
            {
                HtmlEncode(writer, (string)value);
            }
            else
            {
                writer.Write(value);
            }
        }

        public static bool Boolify(object o)
        {
            if (o is bool) return (bool)o;
            return o != null;
        }

        private static ConcurrentDictionary<Tuple<Type, string>, Func<object, object>> lateBoundCache = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();

        public static object RuntimeBind(object model, string itemName, bool isCaseSensitive)
        {
            var binder = lateBoundCache.GetOrAdd(Tuple.Create(model.GetType(), itemName), new Func<Tuple<Type, string>, Func<object, object>>(pair =>
            {
                var type = pair.Item1;
                var flags = GetBindingFlags(isCaseSensitive);

                if (pair.Item2.EndsWith("()"))
                {
                    var function = type.GetMethod(pair.Item2.Substring(0, pair.Item2.Length - 2), flags, null, new Type[0], new ParameterModifier[0]);
                    if (function != null) return DelegateBuilder.FunctionCall(type, function);
                }

                var property = type.GetProperty(pair.Item2, flags);
                if (property != null) return DelegateBuilder.Property(type, property);

                var field = type.GetField(pair.Item2, flags);
                if (field != null) return DelegateBuilder.Field(type, field);

                var dictionaryType = type.GetDictionaryTypeWithKey<string>();
                if (dictionaryType != null) return DelegateBuilder.Dictionary(dictionaryType, pair.Item2);

                return null;
            }));

            if (binder == null) throw new VeilCompilerException("Unable to late-bind '{0}' against model {1}".FormatInvariant(itemName, model.GetType().Name));
            var result = binder(model);
            return result;
        }

        private static BindingFlags GetBindingFlags(bool isCaseSensitive)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (!isCaseSensitive)
            {
                flags = flags | BindingFlags.IgnoreCase;
            }
            return flags;
        }
    }
}