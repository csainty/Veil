using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal static class HandlebarsExpressionParser
    {
        public static SyntaxTreeNode.ExpressionNode Parse(LinkedList<HandlebarsParser.ParserScope> scopes, string expression)
        {
            expression = expression.Trim();

            if (expression == "this")
            {
                return SyntaxTreeNode.ExpressionNode.Self(scopes.First().ModelInScope, SyntaxTreeNode.ExpressionScope.CurrentModelOnStack);
            }
            if (expression.StartsWith("../"))
            {
                return ParseAgainstModel(scopes.First.Next.Value.ModelInScope, expression.Substring(3), SyntaxTreeNode.ExpressionScope.ModelOfParentScope);
            }

            return ParseAgainstModel(scopes.First().ModelInScope, expression, SyntaxTreeNode.ExpressionScope.CurrentModelOnStack);
        }

        private static SyntaxTreeNode.ExpressionNode ParseAgainstModel(Type modelType, string expression, SyntaxTreeNode.ExpressionNode.ExpressionScope expressionScope)
        {
            var dotIndex = expression.IndexOf('.');
            if (dotIndex >= 0)
            {
                var subModel = HandlebarsExpressionParser.ParseAgainstModel(modelType, expression.Substring(0, dotIndex), expressionScope);
                return SyntaxTreeNode.ExpressionNode.SubModel(
                    subModel,
                    HandlebarsExpressionParser.ParseAgainstModel(subModel.ResultType, expression.Substring(dotIndex + 1), SyntaxTreeNode.ExpressionScope.CurrentModelOnStack)
                );
            }

            if (expression.EndsWith("()"))
            {
                var methodInfo = modelType.GetMethod(expression.Substring(0, expression.Length - 2));
                if (methodInfo != null) return SyntaxTreeNode.ExpressionNode.Function(modelType, expression.Substring(0, expression.Length - 2), expressionScope);
            }

            var propertyInfo = modelType.GetProperty(expression);
            if (propertyInfo != null) return SyntaxTreeNode.ExpressionNode.Property(modelType, expression, expressionScope);

            var fieldInfo = modelType.GetField(expression);
            if (fieldInfo != null) return SyntaxTreeNode.ExpressionNode.Field(modelType, expression, expressionScope);

            if (IsLateBoundAcceptingType(modelType)) return SyntaxTreeNode.ExpressionNode.LateBound(expression);

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