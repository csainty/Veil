using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal static class HandlebarsExpressionParser
    {
        public static ExpressionNode Parse(HandlebarsScopeStack scopes, string expression)
        {
            expression = expression.Trim();

            if (expression == "this")
            {
                return Expression.Self(scopes.GetTypeOfModelInScope(), ExpressionScope.CurrentModelOnStack);
            }
            if (expression.StartsWith("../"))
            {
                return ParseAgainstModel(scopes.GetTypeOfParentScopeModel(), expression.Substring(3), ExpressionScope.ModelOfParentScope);
            }

            return ParseAgainstModel(scopes.GetTypeOfModelInScope(), expression, ExpressionScope.CurrentModelOnStack);
        }

        private static ExpressionNode ParseAgainstModel(Type modelType, string expression, ExpressionScope expressionScope)
        {
            var dotIndex = expression.IndexOf('.');
            if (dotIndex >= 0)
            {
                var subModel = HandlebarsExpressionParser.ParseAgainstModel(modelType, expression.Substring(0, dotIndex), expressionScope);
                return Expression.SubModel(
                    subModel,
                    HandlebarsExpressionParser.ParseAgainstModel(subModel.ResultType, expression.Substring(dotIndex + 1), ExpressionScope.CurrentModelOnStack)
                );
            }

            if (expression.EndsWith("()"))
            {
                var methodInfo = modelType.GetMethod(expression.Substring(0, expression.Length - 2));
                if (methodInfo != null) return Expression.Function(modelType, expression.Substring(0, expression.Length - 2), expressionScope);
            }

            var propertyInfo = modelType.GetProperty(expression);
            if (propertyInfo != null) return Expression.Property(modelType, expression, expressionScope);

            var fieldInfo = modelType.GetField(expression);
            if (fieldInfo != null) return Expression.Field(modelType, expression, expressionScope);

            if (IsLateBoundAcceptingType(modelType)) return Expression.LateBound(expression);

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}' againt model '{1}'", expression, modelType.Name));
        }

        private static bool IsLateBoundAcceptingType(Type type)
        {
            return type == typeof(object) || (type.IsDictionary() || type.GetInterfaces().Any(IsDictionary));
        }

        private static bool IsDictionary(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }
    }
}