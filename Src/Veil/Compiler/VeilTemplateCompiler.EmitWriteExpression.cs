using System.Reflection;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static MethodInfo htmlEncodeMethod = typeof(Helpers).GetMethod("HtmlEncode");

        private void EmitWriteExpression(SyntaxTreeNode.WriteExpressionNode node)
        {
            emitter.LoadWriterToStack();
            PushExpressionScopeOnStack(node.Expression);
            emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);

            if (node.HtmlEncode)
            {
                if (node.Expression.ResultType != typeof(string)) throw new VeilCompilerException("Tried to HtmlEncode an expression that does not evaluate to a string");
                emitter.Call(htmlEncodeMethod);
            }
            else
            {
                emitter.CallWriteFor(node.Expression.ResultType);
            }
        }
    }
}