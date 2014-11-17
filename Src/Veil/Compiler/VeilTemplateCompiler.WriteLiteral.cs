using System.Linq.Expressions;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression WriteLiteral(WriteLiteralNode node)
        {
            return Expression.Call(this.writer, writeMethod, Expression.Constant(node.LiteralContent, typeof(string)));
        }
    }
}