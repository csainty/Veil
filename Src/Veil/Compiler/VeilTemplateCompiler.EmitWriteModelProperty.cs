using System;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitWriteModelProperty<T>(Emit<Action<TextWriter, T>> emitter, WriteModelExpressionNode node)
        {
            emitter.LoadWriterToStack();
            emitter.LoadModelExpressionToStack(node.Expression);
            emitter.CallWriteFor(node.Expression.Type);
        }
    }
}