using System;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitWriteModelProperty<T>(VeilCompilerState<T> state, WriteModelExpressionNode node)
        {
            state.Emitter.LoadWriterToStack();
            state.PushCurrentModelOnStack();
            state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
            state.Emitter.CallWriteFor(node.Expression.Type);
        }
    }
}