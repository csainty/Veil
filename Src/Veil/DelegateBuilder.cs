using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Veil
{
    internal static class DelegateBuilder
    {
        public static Func<object, object> FunctionCall(Type modelType, MethodInfo function)
        {
            var method = new DynamicMethod("DelegateBuilder_FunctionCall_{0}_{1}".FormatInvariant(modelType.Name, function.Name), typeof(object), new[] { typeof(object) }, true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, modelType);

            if (function.IsVirtual)
            {
                il.Emit(OpCodes.Callvirt, function);
            }
            else
            {
                il.Emit(OpCodes.Call, function);
            }
            if (function.ReturnType.IsValueType)
            {
                il.Emit(OpCodes.Box, function.ReturnType);
            }
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        public static Func<object, object> Property(Type modelType, PropertyInfo property)
        {
            var getter = property.GetGetMethod();

            var method = new DynamicMethod("DelegateBuilder_Property_{0}_{1}".FormatInvariant(modelType.Name, property.Name), typeof(object), new[] { typeof(object) }, true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, modelType);

            if (getter.IsVirtual)
            {
                il.Emit(OpCodes.Callvirt, getter);
            }
            else
            {
                il.Emit(OpCodes.Call, getter);
            }

            if (property.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, property.PropertyType);
            }
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        public static Func<object, object> Field(Type modelType, FieldInfo field)
        {
            var method = new DynamicMethod("DelegateBuilder_Field_{0}_{1}".FormatInvariant(modelType.Name, field.Name), typeof(object), new[] { typeof(object) }, true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, modelType);
            il.Emit(OpCodes.Ldfld, field);
            if (field.FieldType.IsValueType) il.Emit(OpCodes.Box, field.FieldType);
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        public static Func<object, object> Dictionary(Type modelType, string key)
        {
            var getItem = modelType.GetMethod("get_Item");
            var method = new DynamicMethod("DelegateBuilder_Dictionary_{0}_{1}".FormatInvariant(modelType.Name, key), typeof(object), new[] { typeof(object) }, true);
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