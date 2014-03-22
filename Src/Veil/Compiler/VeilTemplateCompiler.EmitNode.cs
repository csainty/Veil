using System;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitNode<T>(Emit<Action<TextWriter, T>> emitter, ISyntaxTreeNode node)
        {
            var nodeType = node.GetType();
            if (nodeType == typeof(WriteLiteralNode))
            {
                EmitWriteLiteral(emitter, (WriteLiteralNode)node);
            }
            else
            {
                throw new VeilCompilerException("Unknown SyntaxTreeNode {0}".FormatInvariant(nodeType.Name));
            }
        }
    }
}