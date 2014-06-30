using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleExpressionParser
    {
        public static ExpressionNode Parse(LinkedList<SuperSimpleParserScope> scopes, string originalExpression)
        {
            var expression = originalExpression.Trim();

            if (expression == "Model")
            {
                return Expression.Self(scopes.Last.Value.ModelType, ExpressionScope.RootModel);
            }
            if (expression == "Current")
            {
                return Expression.Self(scopes.First.Value.ModelType, ExpressionScope.CurrentModelOnStack);
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

        private static ExpressionNode ParseAgainstModel(string originalExpression, string expression, SuperSimpleParserScope scope, ExpressionScope expressionScope)
        {
            var subModelIndex = expression.IndexOf('.');
            if (subModelIndex >= 0)
            {
                var subModel = ParseAgainstModel(originalExpression, expression.Substring(0, subModelIndex), scope, expressionScope);
                return Expression.SubModel(
                    subModel,
                    ParseAgainstModel(originalExpression, expression.Substring(subModelIndex + 1), new SuperSimpleParserScope { Block = scope.Block, ModelType = subModel.ResultType }, ExpressionScope.CurrentModelOnStack)
                );
            }

            var propertyInfo = scope.ModelType.GetProperty(expression);
            if (propertyInfo != null) return Expression.Property(scope.ModelType, expression, expressionScope);

            var fieldInfo = scope.ModelType.GetField(expression);
            if (fieldInfo != null) return Expression.Field(scope.ModelType, expression, expressionScope);

            if (expression.StartsWith("Has"))
            {
                var collectionExpression = ParseAgainstModel(originalExpression, expression.Substring(3), scope, expressionScope);
                return Expression.HasItems(collectionExpression);
            }

            if (IsLateBoundAcceptingType(scope.ModelType)) return Expression.LateBound(expression, expressionScope);

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}'", originalExpression));
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