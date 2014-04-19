using System;

namespace Veil.SuperSimple
{
    public static class SuperSimpleExpressionParser
    {
        public static SyntaxTreeNode.ExpressionNode Parse(Type modelType, string expression)
        {
            expression = expression.Trim();

            if (expression == "Model" || expression == "Current")
            {
                return SyntaxTreeNode.ExpressionNode.Self(modelType);
            }

            if (expression.StartsWith("Model."))
            {
                expression = expression.Substring(6);
            }

            var propertyInfo = modelType.GetProperty(expression);
            if (propertyInfo != null) return SyntaxTreeNode.ExpressionNode.Property(modelType, expression);

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}' againt model '{1}'", expression, modelType.Name));
        }
    }
}