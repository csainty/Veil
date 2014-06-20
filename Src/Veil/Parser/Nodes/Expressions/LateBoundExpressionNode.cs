using System;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates its type at run time using only its name
    /// </summary>
    public class LateBoundExpressionNode : ExpressionNode
    {
        public string ItemName { get; set; }

        public override Type ResultType
        {
            get { return typeof(object); }
        }
    }
}