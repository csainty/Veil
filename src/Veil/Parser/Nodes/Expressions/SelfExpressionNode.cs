using System;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates the model itself
    /// </summary>
    public class SelfExpressionNode : ExpressionNode
    {
        /// <summary>
        /// The type of the model which is referencing itself
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// The type of result from this expression evaluation
        /// </summary>
        public override Type ResultType { get { return this.ModelType; } }
    }
}