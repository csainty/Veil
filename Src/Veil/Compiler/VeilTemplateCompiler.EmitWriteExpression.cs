using System.Reflection;
using Veil.Parser;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static MethodInfo htmlEncodeMethod = typeof(Helpers).GetMethod("HtmlEncode");
        private static MethodInfo toStringMethod = typeof(object).GetMethod("ToString");

        private void EmitWriteExpression(SyntaxTreeNode.WriteExpressionNode node)
        {
            LoadWriterToStack();
            EvaluateExpression(node.Expression);

            var valueType = node.Expression.ResultType;
            if (!writers.ContainsKey(valueType))
            {
                valueType = typeof(string);
                emitter.CallMethod(toStringMethod);
            }

            if (node.HtmlEncode)
            {
                if (valueType != typeof(string)) throw new VeilCompilerException("Tried to HtmlEncode an expression that does not evaluate to a string");
                emitter.Call(htmlEncodeMethod);
            }
            else
            {
                CallWriteFor(valueType);
            }
        }
    }
}