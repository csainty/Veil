using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleInclude(IncludeTemplateNode node)
        {
            var includeModel = ParseExpression(node.ModelExpression);
            var template = this.includeParser(node.TemplateName, includeModel.Type);
            if (template == null) throw new VeilCompilerException("Unable to load template '{0}'".FormatInvariant(node.TemplateName));

            var storedModel = Expression.Variable(includeModel.Type);
            using (CreateLocalModelStack())
            {
                PushScope(storedModel);
                var body = HandleNode(template);
                PopScope();

                return Expression.Block(
                    new[] { storedModel },
                    Expression.Assign(storedModel, includeModel),
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