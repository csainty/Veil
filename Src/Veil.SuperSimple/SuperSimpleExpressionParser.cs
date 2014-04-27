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

            var propertyInfo = chosenScope.ModelType.GetProperty(expression);
            if (propertyInfo != null) return SyntaxTreeNode.ExpressionNode.Property(chosenScope.ModelType, expression, expressionScope);

            if (expression.StartsWith("Has"))
            {
                var collectionExpression = Parse(CreateSingleScope(chosenScope), expression.Substring(3));
                return SyntaxTreeNode.ExpressionNode.HasItems(collectionExpression);
            }

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}'", originalExpression));
        }

        private static LinkedList<SuperSimpleParser.ParserScope> CreateSingleScope(SuperSimpleParser.ParserScope scope)
        {
            return new LinkedList<SuperSimpleParser.ParserScope>(new[] { scope });
        }
    }
}