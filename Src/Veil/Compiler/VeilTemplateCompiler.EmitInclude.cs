using Veil.Parser;

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
                EvaluateExpression(node.ModelExpression);
                emitter.StoreLocal(model);

                using (CreateLocalScopeStack())
                {
                    AddModelScope(e => e.LoadLocal(model));

                    EmitNode(template);

                    RemoveModelScope();
                }
            }
        }
    }
}