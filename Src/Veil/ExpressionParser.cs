using System;
using Veil.Parser;

namespace Veil
{
    internal static class ExpressionParser
    {
        public static IModelPropertyNode Parse(Type modelType, string expression)
        {
            expression = expression.Trim();
            var propertyInfo = modelType.GetProperty(expression);
            if (propertyInfo == null) throw new VeilParserException("Unable to parse model expression '{0}' againt model '{1}'".FormatInvariant(expression, modelType.Name));
            return new ModelProperty { Property = propertyInfo };
        }
    }
}