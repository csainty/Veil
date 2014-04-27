namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitWriteExpression<T>(VeilCompilerState<T> state, SyntaxTreeNode.WriteExpressionNode node)
        {
            state.Emitter.LoadWriterToStack();
            state.PushExpressionScopeOnStack(node.Expression);
            state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);

            if (node.HtmlEncode && node.Expression.ResultType == typeof(string))
            {
                var method = typeof(Helpers).GetMethod("HtmlEncode");
                state.Emitter.Call(method);
            }
            else
            {
                state.Emitter.CallWriteFor(node.Expression.ResultType);
            }
        }
    }
}