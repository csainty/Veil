using System;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitWriteLiteral<T>(VeilCompilerState<T> state, SyntaxTreeNode.WriteLiteralNode node)
        {
            if (node.LiteralType == typeof(string))
            {
                state.Emitter.OutputLiteral((string)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(int))
            {
                state.Emitter.OutputLiteral((int)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(double))
            {
                state.Emitter.OutputLiteral((double)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(float))
            {
                state.Emitter.OutputLiteral((float)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(long))
            {
                state.Emitter.OutputLiteral((long)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(uint))
            {
                state.Emitter.OutputLiteral((uint)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(ulong))
            {
                state.Emitter.OutputLiteral((ulong)node.LiteralContent);
            }
            else
            {
                throw new VeilCompilerException("Unable to write literal of type {0}".FormatInvariant(node.LiteralType.Name));
            }
        }
    }
}