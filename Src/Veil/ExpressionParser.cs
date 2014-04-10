using System;
using Veil.Parser;

namespace Veil
{
    internal static class ExpressionParser
    {
        public static IModelExpressionNode Parse(Type modelType, string expression)
        {
            expression = expression.Trim();

            var propertyInfo = modelType.GetProperty(expression);
            if (propertyInfo != null) return new ModelPropertyExpressionNode { Property = propertyInfo };

            var fieldInfo = modelType.GetField(expression);
            if (fieldInfo != null) return new ModelFieldExpressionNode { Field = fieldInfo };

            throw new VeilParserException("Unable to parse model expression '{0}' againt model '{1}'".FormatInvariant(expression, modelType.Name));
        }
    }
}