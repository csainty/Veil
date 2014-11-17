using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression Include(Parser.Nodes.IncludeTemplateNode node)
        {
            var template = includeParser(node.TemplateName, node.ModelExpression.ResultType);
            if (template == null) throw new VeilCompilerException("Unable to load template '{0}'".FormatInvariant(node.TemplateName));

            var local = Expression.Variable(node.ModelExpression.ResultType);
            var model = ParseExpression(node.ModelExpression);

            using (CreateLocalModelStack())
            {
                PushScope(local);
                var body = Node(template);
                PopScope();

                return Expression.Block(
                    new[] { local },
                    Expression.Assign(local, model.Expression),
                    body

                );
            }
        }

        private IDisposable CreateLocalModelStack()
        {
            var oldScopeStack = this.modelStack;
            this.modelStack = new LinkedList<Expression>();
            return new ActionDisposable(() =>
            {
                this.modelStack = oldScopeStack;
            });
        }
    }
}