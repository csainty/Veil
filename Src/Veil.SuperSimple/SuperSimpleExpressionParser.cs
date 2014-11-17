using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleExpressionParser
    {
        public static ExpressionNode Parse(LinkedList<SuperSimpleTemplateParserScope> scopes, string originalExpression)
        {
            var expression = originalExpression.Trim();

            if (expression == "Model")
            {
                return SyntaxTreeExpression.Self(scopes.Last.Value.ModelType, ExpressionScope.RootModel);
            }
            if (expression == "Current")
            {
                return SyntaxTreeExpression.Self(scopes.First.Value.ModelType, ExpressionScope.CurrentModelOnStack);
            }

            var chosenScope = scopes.First.Value;
            var expressionScope = ExpressionScope.CurrentModelOnStack;

            if (expression.StartsWith("Current."))
            {
                expression = expression.Substring(8);
            }
            else if (expression.StartsWith("Model."))
            {
                expression = expression.Substring(6);
                chosenScope = scopes.Last.Value;
                expressionScope = ExpressionScope.RootModel;
            }

            return ParseAgainstModel(originalExpression, expression, chosenScope, expressionScope);
        }

        private static ExpressionNode ParseAgainstModel(string originalExpression, string expression, SuperSimpleTemplateParserScope scope, ExpressionScope expressionScope)
        {
            var subModelIndex = expression.IndexOf('.');
            if (subModelIndex >= 0)
            {
                var subModel = ParseAgainstModel(originalExpression, expression.Substring(0, subModelIndex), scope, expressionScope);
                return SyntaxTreeExpression.SubModel(
                    subModel,
                    ParseAgainstModel(originalExpression, expression.Substring(subModelIndex + 1), new SuperSimpleTemplateParserScope { Block = scope.Block, ModelType = subModel.ResultType }, ExpressionScope.CurrentModelOnStack)
                );
            }

            var propertyInfo = scope.ModelType.GetProperty(expression);
            if (propertyInfo != null) return SyntaxTreeExpression.Property(scope.ModelType, expression, expressionScope);

            var fieldInfo = scope.ModelType.GetField(expression);
            if (fieldInfo != null) return SyntaxTreeExpression.Field(scope.ModelType, expression, expressionScope);

            if (IsLateBoundAcceptingType(scope.ModelType)) return SyntaxTreeExpression.LateBound(expression, true, expressionScope);

            if (expression.StartsWith("Has"))
            {
                var collectionExpression = ParseAgainstModel(originalExpression, expression.Substring(3), scope, expressionScope);
                return SyntaxTreeExpression.HasItems(collectionExpression);
            }

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}'", originalExpression));
        }

        private static bool IsLateBoundAcceptingType(Type type)
        {
            return type == typeof(object)
                || type.IsDictionary()
                || type.GetInterfaces().Any(IsDictionary)
                || type.GetProperties().Any(p => p.GetIndexParameters().Any());
        }

        private static bool IsDictionary(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }
    }
}