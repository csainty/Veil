using System;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates the model itself
    /// </summary>
    public class SelfExpressionNode : ExpressionNode
    {
        public Type ModelType { get; set; }

        public override Type ResultType { get { return this.ModelType; } }
    }
}