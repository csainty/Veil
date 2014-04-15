using System;

namespace Veil
{
    internal static class ExpressionParser
    {
        public static SyntaxTreeNode.ExpressionNode Parse(Type modelType, string expression)
        {
            expression = expression.Trim();

            var dotIndex = expression.IndexOf('.');
            if (dotIndex >= 0)
            {
                var subModel = ExpressionParser.Parse(modelType, expression.Substring(0, dotIndex));
                return SyntaxTreeNode.ExpressionNode.ModelSubModel(
                    subModel,
                    ExpressionParser.Parse(subModel.ResultType, expression.Substring(dotIndex + 1))
                );
            }

            if (expression.EndsWith("()"))
            {
                var methodInfo = modelType.GetMethod(expression.Substring(0, expression.Length - 2));
                if (methodInfo != null) return new SyntaxTreeNode.ExpressionNode.FunctionCallExpressionNode { Function = methodInfo };
            }

            if (expression == "this")
            {
                return SyntaxTreeNode.ExpressionNode.Self(modelType);
            }

            var propertyInfo = modelType.GetProperty(expression);
            if (propertyInfo != null) return new SyntaxTreeNode.ExpressionNode.ModelPropertyExpressionNode { Property = propertyInfo };

            var fieldInfo = modelType.GetField(expression);
            if (fieldInfo != null) return new SyntaxTreeNode.ExpressionNode.ModelFieldExpressionNode { Field = fieldInfo };

            throw new VeilParserException("Unable to parse model expression '{0}' againt model '{1}'".FormatInvariant(expression, modelType.Name));
        }
    }
}