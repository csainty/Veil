using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression Node(Parser.SyntaxTreeNode node)
        {
            if (node is Parser.Nodes.BlockNode) return Block((Parser.Nodes.BlockNode)node);
            if (node is Parser.Nodes.WriteLiteralNode) return WriteLiteral((Parser.Nodes.WriteLiteralNode)node);
            if (node is Parser.Nodes.WriteExpressionNode) return WriteExpression((Parser.Nodes.WriteExpressionNode)node);
            if (node is Parser.Nodes.IterateNode) return Iterate((Parser.Nodes.IterateNode)node);
            if (node is Parser.Nodes.ConditionalNode) return Conditional((Parser.Nodes.ConditionalNode)node);
            if (node is Parser.Nodes.ScopedNode) return ScopedNode((Parser.Nodes.ScopedNode)node);
            if (node is Parser.Nodes.FlushNode) return Flush();
            if (node is Parser.Nodes.IncludeTemplateNode) return Include((Parser.Nodes.IncludeTemplateNode)node);
            if (node is Parser.Nodes.OverridePointNode) return Override((Parser.Nodes.OverridePointNode)node);

            throw new VeilCompilerException("Unknown SyntaxTreeNode {0}".FormatInvariant(node.GetType().Name));
        }
    }
}