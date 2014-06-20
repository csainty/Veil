using System;

namespace Veil.Parser
{
    /// <summary>
    /// Abstract base class for nodes in hte syntax tree which evaluate expressions
    /// </summary>
    public abstract class ExpressionNode : SyntaxTreeNode
    {
        public ExpressionScope Scope { get; set; }

        public abstract Type ResultType { get; }
    }
}