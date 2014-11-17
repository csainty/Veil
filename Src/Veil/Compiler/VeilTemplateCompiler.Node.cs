using System.Linq.Expressions;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression Node(SyntaxTreeNode node)
        {
            if (node is BlockNode) return Block((BlockNode)node);
            if (node is WriteLiteralNode) return WriteLiteral((WriteLiteralNode)node);
            if (node is WriteExpressionNode) return WriteExpression((WriteExpressionNode)node);
            if (node is IterateNode) return Iterate((IterateNode)node);
            if (node is ConditionalNode) return Conditional((ConditionalNode)node);
            if (node is ScopedNode) return ScopedNode((ScopedNode)node);
            if (node is FlushNode) return Flush();
            if (node is IncludeTemplateNode) return Include((IncludeTemplateNode)node);
            if (node is OverridePointNode) return Override((OverridePointNode)node);

            throw new VeilCompilerException("Unknown SyntaxTreeNode {0}".FormatInvariant(node.GetType().Name));
        }
    }
}