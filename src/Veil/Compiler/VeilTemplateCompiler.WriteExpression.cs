using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo writeMethod = typeof(TextWriter).GetMethod("Write", new[] { typeof(string) });
        private static readonly MethodInfo encodeMethod = typeof(Helpers).GetMethod("HtmlEncode");

        private Expression HandleWriteExpression(WriteExpressionNode node)
        {
            var expression = ParseExpression(node.Expression);

            if (expression.Type != typeof(string))
            {
                var toStringMethod = expression.Type.GetMethod("ToString", new Type[0]);
                expression = Expression.Call(expression, toStringMethod);
            }
            if (node.HtmlEncode)
            {
                return Expression.Call(encodeMethod, this.writer, expression);
            }

            return Expression.Call(this.writer, writeMethod, expression);
        }
    }
}