using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo writeMethod = typeof(TextWriter).GetMethod("Write", new[] { typeof(string) });

        private Expression WriteExpression(Parser.Nodes.WriteExpressionNode node)
        {
            var expression = ParseExpression(node.Expression);

            var ex = expression.Expression;
            if (expression.Type != typeof(string))
            {
                var toStringMethod = expression.Type.GetMethod("ToString", new Type[0]);
                ex = Expression.Call(expression.Expression, toStringMethod);
            }
            if (node.HtmlEncode)
            {
                var encodeMethod = typeof(Helpers).GetMethod("HtmlEncode");
                return Expression.Call(encodeMethod, writer, ex);
            }

            return Expression.Call(writer, writeMethod, ex);
        }
    }
}