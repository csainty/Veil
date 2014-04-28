using System;
using System.Collections.Generic;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleExpressionParser
    {
        public static SyntaxTreeNode.ExpressionNode Parse(LinkedList<SuperSimpleParser.ParserScope> scopes, string originalExpression)
        {
            var expression = originalExpression.Trim();

            if (expression == "Model")
            {
                return SyntaxTreeNode.ExpressionNode.Self(scopes.Last.Value.ModelType, SyntaxTreeNode.ExpressionScope.RootModel);
            }
            if (expression == "Current")
            {
                return SyntaxTreeNode.ExpressionNode.Self(scopes.First.Value.ModelType, SyntaxTreeNode.ExpressionScope.CurrentModelOnStack);
            }

            var chosenScope = scopes.First.Value;
            var expressionScope = SyntaxTreeNode.ExpressionScope.CurrentModelOnStack;

            if (expression.StartsWith("Current."))
            {
                expression = expression.Substring(8);
            }
            else if (expression.StartsWith("Model."))
            {
                expression = expression.Substring(6);
                chosenScope = scopes.Last.Value;
                expressionScope = SyntaxTreeNode.ExpressionScope.RootModel;
            }

            return ParseAgainstModel(originalExpression, expression, chosenScope, expressionScope);
        }

        private static SyntaxTreeNode.ExpressionNode ParseAgainstModel(string originalExpression, string expression, SuperSimpleParser.ParserScope scope, SyntaxTreeNode.ExpressionScope expressionScope)
        {
            var subModelIndex = expression.IndexOf('.');
            if (subModelIndex >= 0)
            {
                var subModel = ParseAgainstModel(originalExpression, expression.Substring(0, subModelIndex), scope, expressionScope);
                return SyntaxTreeNode.ExpressionNode.SubModel(
                    subModel,
                    ParseAgainstModel(originalExpression, expression.Substring(subModelIndex + 1), new SuperSimpleParser.ParserScope { Block = scope.Block, ModelType = subModel.ResultType }, SyntaxTreeNode.ExpressionScope.CurrentModelOnStack)
                );
            }

            var propertyInfo = scope.ModelType.GetProperty(expression);
            if (propertyInfo != null) return SyntaxTreeNode.ExpressionNode.Property(scope.ModelType, expression, expressionScope);

            var fieldInfo = scope.ModelType.GetField(expression);
            if (fieldInfo != null) return SyntaxTreeNode.ExpressionNode.Field(scope.ModelType, expression, expressionScope);

            if (expression.StartsWith("Has"))
            {
                var collectionExpression = ParseAgainstModel(originalExpression, expression.Substring(3), scope, expressionScope);
                return SyntaxTreeNode.ExpressionNode.HasItems(collectionExpression);
            }

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}'", originalExpression));
        }
    }
}