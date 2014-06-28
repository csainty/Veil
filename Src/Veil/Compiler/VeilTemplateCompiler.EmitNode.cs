using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitNode(SyntaxTreeNode node)
        {
            var nodeType = node.GetType();
            if (nodeType == typeof(WriteLiteralNode))
                EmitWriteLiteral((WriteLiteralNode)node);
            else if (nodeType == typeof(WriteExpressionNode))
                EmitWriteExpression((WriteExpressionNode)node);
            else if (nodeType == typeof(ConditionalNode))
                EmitConditional((ConditionalNode)node);
            else if (nodeType == typeof(IterateNode))
                EmitIterate((IterateNode)node);
            else if (nodeType == typeof(BlockNode))
                EmitBlock((BlockNode)node);
            else if (nodeType == typeof(IncludeTemplateNode))
                EmitInclude((IncludeTemplateNode)node);
            else if (nodeType == typeof(OverridePointNode))
                EmitOverride((OverridePointNode)node);
            else if (nodeType == typeof(FlushNode))
                EmitFlush();
            else if (nodeType == typeof(ScopedNode))
                EmitScopedNode((ScopedNode)node);
            else if (nodeType == typeof(ExtendTemplateNode))
                throw new VeilCompilerException("Found an ExtendTemplate node inside a SyntaxTree. Extend nodes must be the root of a tree.");
            else
                throw new VeilCompilerException("Unknown SyntaxTreeNode {0}".FormatInvariant(nodeType.Name));
        }
    }
}