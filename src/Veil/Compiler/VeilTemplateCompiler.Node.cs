using System.Linq.Expressions;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleNode(SyntaxTreeNode node)
        {
            if (node is BlockNode) return HandleBlock((BlockNode)node);
            if (node is WriteLiteralNode) return HandleWriteLiteral((WriteLiteralNode)node);
            if (node is WriteExpressionNode) return HandleWriteExpression((WriteExpressionNode)node);
            if (node is IterateNode) return HandleIterate((IterateNode)node);
            if (node is ConditionalNode) return HandleConditional((ConditionalNode)node);
            if (node is ScopedNode) return HandleScopedNode((ScopedNode)node);
            if (node is FlushNode) return HandleFlush();
            if (node is IncludeTemplateNode) return HandleInclude((IncludeTemplateNode)node);
            if (node is OverridePointNode) return HandleOverride((OverridePointNode)node);
            if (node is ExtendTemplateNode) throw new VeilCompilerException("Found an ExtendTemplate node inside a SyntaxTree. Extend nodes must be the root of a tree.");

            throw new VeilCompilerException("Unknown SyntaxTreeNode {0}".FormatInvariant(node.GetType().Name));
        }
    }
}