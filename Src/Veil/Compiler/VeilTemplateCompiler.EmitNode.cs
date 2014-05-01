using Veil.Parser;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitNode(SyntaxTreeNode node)
        {
            var nodeType = node.GetType();
            if (nodeType == typeof(SyntaxTreeNode.WriteLiteralNode))
                EmitWriteLiteral((SyntaxTreeNode.WriteLiteralNode)node);
            else if (nodeType == typeof(SyntaxTreeNode.WriteExpressionNode))
                EmitWriteExpression((SyntaxTreeNode.WriteExpressionNode)node);
            else if (nodeType == typeof(SyntaxTreeNode.ConditionalNode))
                EmitConditional((SyntaxTreeNode.ConditionalNode)node);
            else if (nodeType == typeof(SyntaxTreeNode.IterateNode))
                EmitIterate((SyntaxTreeNode.IterateNode)node);
            else if (nodeType == typeof(SyntaxTreeNode.BlockNode))
                EmitBlock((SyntaxTreeNode.BlockNode)node);
            else if (nodeType == typeof(SyntaxTreeNode.IncludeTemplateNode))
                EmitInclude((SyntaxTreeNode.IncludeTemplateNode)node);
            else
                throw new VeilCompilerException("Unknown SyntaxTreeNode {0}".FormatInvariant(nodeType.Name));
        }
    }
}