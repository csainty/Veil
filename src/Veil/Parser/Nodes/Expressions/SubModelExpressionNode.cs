using System;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates another expression against a sub model. Used for traversing
    /// </summary>
    public class SubModelExpressionNode : ExpressionNode
    {
        /// <summary>
        /// An expression which evluates to the parent model object
        /// </summary>
        public ExpressionNode ModelExpression { get; set; }

        /// <summary>
        /// An expression evaluated in the scope of <see cref="ModelExpression"/>
        /// </summary>
        public ExpressionNode SubModelExpression { get; set; }

        /// <summary>
        /// The type of result from this expression evaluation
        /// </summary>
        public override Type ResultType
        {
            get { return SubModelExpression.ResultType; }
        }
    }
}