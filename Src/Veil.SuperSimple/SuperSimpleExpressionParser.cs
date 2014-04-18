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
            if (propertyInfo != null) return new SyntaxTreeNode.ExpressionNode.ModelPropertyExpressionNode { Property = propertyInfo };

            return null;
        }
    }
}