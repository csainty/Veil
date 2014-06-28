using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitScopedNode(ScopedNode node)
        {
            using (var model = emitter.DeclareLocal(node.ModelToScope.ResultType))
            {
                EvaluateExpression(node.ModelToScope);
                emitter.StoreLocal(model);
                AddModelScope(x => x.LoadLocal(model));
                EmitNode(node.Node);
                RemoveModelScope();
            }
        }
    }
}