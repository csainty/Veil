using System.Collections.Generic;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitNode(SyntaxTreeNode rootNode)
        {
            var nodes = GetIndividualNodes(rootNode);
            foreach (var node in nodes)
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
                else
                    throw new VeilCompilerException("Unknown SyntaxTreeNode {0}".FormatInvariant(nodeType.Name));
            }
        }

        private static IEnumerable<SyntaxTreeNode> GetIndividualNodes(SyntaxTreeNode node)
        {
            if (node is Veil.SyntaxTreeNode.BlockNode)
                return ((Veil.SyntaxTreeNode.BlockNode)node).Nodes;
            else
                return new[] { node };
        }
    }
}