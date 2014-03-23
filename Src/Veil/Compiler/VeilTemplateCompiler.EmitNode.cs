using System;
using System.Collections.Generic;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitNode<T>(Emit<Action<TextWriter, T>> emitter, ISyntaxTreeNode rootNode)
        {
            var nodes = GetIndividualNodes(rootNode);
            foreach (var node in nodes)
            {
                var nodeType = node.GetType();
                if (nodeType == typeof(WriteLiteralNode))
                    EmitWriteLiteral(emitter, (WriteLiteralNode)node);
                else if (nodeType == typeof(WriteModelPropertyNode))
                    EmitWriteModelProperty(emitter, (WriteModelPropertyNode)node);
                else if (nodeType == typeof(ConditionalOnModelPropertyNode))
                    EmitConditionalOnModelProperty(emitter, (ConditionalOnModelPropertyNode)node);
                else
                    throw new VeilCompilerException("Unknown SyntaxTreeNode {0}".FormatInvariant(nodeType.Name));
            }
        }

        private static IEnumerable<ISyntaxTreeNode> GetIndividualNodes(ISyntaxTreeNode node)
        {
            if (node is BlockNode)
                return ((BlockNode)node).TemplateNodes;
            else
                return new[] { node };
        }
    }
}