using System.Linq.Expressions;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleScopedNode(ScopedNode node)
        {
            var scopedModel = ParseExpression(node.ModelToScope);
            var storedModel = Expression.Variable(scopedModel.Type);
            PushScope(storedModel);
            var body = HandleNode(node.Node);
            PopScope();

            return Expression.Block(
                new[] { storedModel },
                Expression.Assign(storedModel, scopedModel),
                body
            );
        }
    }
}