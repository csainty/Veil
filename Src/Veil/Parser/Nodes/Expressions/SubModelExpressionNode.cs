using System;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates another expression against a sub model. Used for traversing
    /// </summary>
    public class SubModelExpressionNode : ExpressionNode
    {
        public ExpressionNode ModelExpression { get; set; }

        public ExpressionNode SubModelExpression { get; set; }

        public override Type ResultType
        {
            get { return SubModelExpression.ResultType; }
        }
    }
}