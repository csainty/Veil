using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace Veil
{
    internal static class DelegateBuilder
    {
        public static Func<object, object> FunctionCall(Type modelType, MethodInfo function)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Call(castModel, function);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Property(Type modelType, PropertyInfo property)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Property(castModel, property);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Field(Type modelType, FieldInfo field)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Field(castModel, field);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Dictionary(Type modelType, string key)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var indexProperty = modelType.GetProperties().First(x => x.GetIndexParameters().Length == 1);
            var call = Expression.MakeIndex(castModel, indexProperty, new[] { Expression.Constant(key) });

            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }
    }
}