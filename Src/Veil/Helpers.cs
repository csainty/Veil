using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

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

        public static object RuntimeBind(object model, string itemName)
        {
            var binder = lateBoundCache.GetOrAdd(Tuple.Create(model.GetType(), itemName), new Func<Tuple<Type, string>, Func<object, object>>(pair =>
            {
                var type = pair.Item1;

                var property = type.GetProperty(pair.Item2);
                if (property != null) return CreatePropertyAccess(type, property);

                var field = type.GetField(pair.Item2);
                if (field != null) return CreateFieldAccess(type, field);

                var dictionaryType = type.GetDictionaryTypeWithKey<string>();
                if (dictionaryType != null) return CreateDictionaryAccess(dictionaryType, pair.Item2);

                return null;
            }));

            if (binder == null) return null;
            return binder(model);
        }

        private static Func<object, object> CreatePropertyAccess(Type modelType, PropertyInfo property)
        {
            var getter = property.GetGetMethod();

            var method = new DynamicMethod("LateBoundPropertyAccess_{0}_{1}".FormatInvariant(modelType.Name, property.Name), typeof(object), new[] { typeof(object) }, true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, modelType);

            if (getter.IsVirtual)
                il.Emit(OpCodes.Callvirt, getter);
            else
                il.Emit(OpCodes.Call, getter);
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        private static Func<object, object> CreateFieldAccess(Type modelType, FieldInfo field)
        {
            var method = new DynamicMethod("LateBoundFieldAccess_{0}_{1}".FormatInvariant(modelType.Name, field.Name), typeof(object), new[] { typeof(object) }, true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, modelType);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        private static Func<object, object> CreateDictionaryAccess(Type modelType, string key)
        {
            var getItem = modelType.GetMethod("get_Item");
            var method = new DynamicMethod("LateBoundDictionaryAccess_{0}_{1}".FormatInvariant(modelType.Name, key), typeof(object), new[] { typeof(object) }, true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, modelType);
            il.Emit(OpCodes.Ldstr, key);
            if (getItem.IsVirtual)
                il.Emit(OpCodes.Callvirt, getItem);
            else
                il.Emit(OpCodes.Call, getItem);
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }
    }
}