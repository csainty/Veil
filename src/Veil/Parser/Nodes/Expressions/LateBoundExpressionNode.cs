using System;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates its type at run time using only its name
    /// </summary>
    public class LateBoundExpressionNode : ExpressionNode
    {
        /// <summary>
        /// The name of the expression to late-bind
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// The type of result from this expression evaluation
        /// </summary>
        public override Type ResultType
        {
            get { return typeof(object); }
        }

        /// <summary>
        /// Indiicates whether the expression should be evaluated with case sensitivity
        /// </summary>
        public bool IsCaseSensitive { get; set; }
    }
}