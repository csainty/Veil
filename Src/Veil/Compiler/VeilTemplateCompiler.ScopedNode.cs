using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression ScopedNode(Parser.Nodes.ScopedNode node)
        {
            var model = ParseExpression(node.ModelToScope);
            var local = Expression.Variable(model.Type);
            PushScope(local);
            var body = Node(node.Node);
            PopScope();

            return Expression.Block(
                new[] { local },
                Expression.Assign(local, model.Expression),
                body
            );
        }
    }
}