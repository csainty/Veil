using System;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitWriteExpression<T>(VeilCompilerState<T> state, SyntaxTreeNode.WriteExpressionNode node)
        {
            state.Emitter.LoadWriterToStack();
            state.PushExpressionScopeOnStack(node.Expression);
            state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
            state.Emitter.CallWriteFor(node.Expression.ResultType);
        }
    }
}