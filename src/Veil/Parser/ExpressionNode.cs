using System;

namespace Veil.Parser
{
    /// <summary>
    /// Abstract base class for nodes in hte syntax tree which evaluate expressions
    /// </summary>
    public abstract class ExpressionNode : SyntaxTreeNode
    {
        /// <summary>
        /// The scope this expression is evaluated in
        /// </summary>
        public ExpressionScope Scope { get; set; }

        /// <summary>
        /// The type of result from this expression evaluation
        /// </summary>
        public abstract Type ResultType { get; }
    }
}