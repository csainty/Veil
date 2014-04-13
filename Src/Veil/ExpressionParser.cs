using System;
using Veil.Parser;

namespace Veil
{
    internal static class ExpressionParser
    {
        public static IModelExpressionNode Parse(Type modelType, string expression)
        {
            expression = expression.Trim();

            var dotIndex = expression.IndexOf('.');
            if (dotIndex >= 0)
            {
                var subModel = ExpressionParser.Parse(modelType, expression.Substring(0, dotIndex));
                return SubModelExpressionNode.Create(
                    subModel,
                    ExpressionParser.Parse(subModel.Type, expression.Substring(dotIndex + 1))
                );
            }

            if (expression.EndsWith("()"))
            {
                var methodInfo = modelType.GetMethod(expression.Substring(0, expression.Length - 2));
                if (methodInfo != null) return new FunctionCallExpressionNode { Function = methodInfo };
            }

            if (expression == "this")
            {
                return SelfExpressionNode.Create(modelType);
            }

            var propertyInfo = modelType.GetProperty(expression);
            if (propertyInfo != null) return new ModelPropertyExpressionNode { Property = propertyInfo };

            var fieldInfo = modelType.GetField(expression);
            if (fieldInfo != null) return new ModelFieldExpressionNode { Field = fieldInfo };

            throw new VeilParserException("Unable to parse model expression '{0}' againt model '{1}'".FormatInvariant(expression, modelType.Name));
        }
    }
}