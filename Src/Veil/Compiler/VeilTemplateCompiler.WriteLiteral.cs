using System.Linq.Expressions;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression WriteLiteral(Parser.Nodes.WriteLiteralNode node)
        {
            return Expression.Call(this.writer, writeMethod, Expression.Constant(node.LiteralContent, typeof(string)));
        }
    }
}