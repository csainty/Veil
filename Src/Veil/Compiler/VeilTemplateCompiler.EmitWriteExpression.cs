using System;
using System.Reflection;
using Veil.Parser;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static MethodInfo htmlEncodeMethod = typeof(Helpers).GetMethod("HtmlEncode");
        private static MethodInfo htmlEncodeLateBoundMethod = typeof(Helpers).GetMethod("HtmlEncodeLateBound");
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

            if (node.HtmlEncode && CanHtmlEncodeType(valueType))
            {
                if (valueType == typeof(string)) emitter.CallMethod(htmlEncodeMethod);
                else emitter.CallMethod(htmlEncodeLateBoundMethod);
            }
            else
            {
                CallWriteFor(valueType);
            }
        }

        private bool CanHtmlEncodeType(Type type)
        {
            return type == typeof(string) || type == typeof(object);
        }
    }
}