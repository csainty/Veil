namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitInclude(SyntaxTreeNode.IncludeTemplateNode node)
        {
            var template = includeParser(node.TemplateName, node.ModelExpression.ResultType);
            if (template == null) throw new VeilCompilerException("Unable to load template '{0}'".FormatInvariant(node.TemplateName));

            using (var model = emitter.DeclareLocal(node.ModelExpression.ResultType))
            {
                PushExpressionScopeOnStack(node.ModelExpression);
                emitter.LoadExpressionFromCurrentModelOnStack(node.ModelExpression);
                emitter.StoreLocal(model);

                var oldScopeStack = scopeStack;
                scopeStack = CreateScopeStack();
                AddModelScope(e => e.LoadLocal(model));

                EmitNode(template);

                RemoveModelScope();
                this.scopeStack = oldScopeStack;
            }
        }
    }
}