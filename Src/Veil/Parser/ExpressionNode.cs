using System;

namespace Veil.Parser
{
    public abstract class ExpressionNode : SyntaxTreeNode
    {
        public ExpressionScope Scope { get; set; }

        public abstract Type ResultType { get; }
    }
}