using System;
using System.Collections.Generic;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleExpressionParser
    {
        public static SyntaxTreeNode.ExpressionNode Parse(LinkedList<SuperSimpleParser.ParserScope> scopes, string expression)
        {
            expression = expression.Trim();
            var rootModelType = scopes.Last.Value.ModelType;
            var scopedModelType = scopes.First.Value.ModelType;

            if (expression == "Model")
            {
                return SyntaxTreeNode.ExpressionNode.Self(rootModelType, SyntaxTreeNode.ExpressionScope.RootModel);
            }
            if (expression == "Current")
            {
                return SyntaxTreeNode.ExpressionNode.Self(scopedModelType, SyntaxTreeNode.ExpressionScope.CurrentModelOnStack);
            }

            if (expression.StartsWith("Model."))
            {
                expression = expression.Substring(6);
                var propertyInfo = rootModelType.GetProperty(expression);
                if (propertyInfo != null) return SyntaxTreeNode.ExpressionNode.Property(rootModelType, expression, SyntaxTreeNode.ExpressionScope.RootModel);
            }
            else
            {
                var propertyInfo = scopedModelType.GetProperty(expression);
                if (propertyInfo != null) return SyntaxTreeNode.ExpressionNode.Property(rootModelType, expression, SyntaxTreeNode.ExpressionScope.CurrentModelOnStack);
            }

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}' againt root model '{1}'", expression, rootModelType.Name));
        }
    }
}