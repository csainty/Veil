using System.Reflection;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static MethodInfo htmlEncodeMethod = typeof(Helpers).GetMethod("HtmlEncode");

        private static void EmitWriteExpression<T>(VeilCompilerState<T> state, SyntaxTreeNode.WriteExpressionNode node)
        {
            state.Emitter.LoadWriterToStack();
            state.PushExpressionScopeOnStack(node.Expression);
            state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);

            if (node.HtmlEncode)
            {
                if (node.Expression.ResultType != typeof(string)) throw new VeilCompilerException("Tried to HtmlEncode an expression that does not evaluate to a string");
                state.Emitter.Call(htmlEncodeMethod);
            }
            else
            {
                state.Emitter.CallWriteFor(node.Expression.ResultType);
            }
        }
    }
}