using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitScopedBlockNode(ScopedBlockNode node)
        {
            using (var model = emitter.DeclareLocal(node.ModelToScope.ResultType))
            {
                EvaluateExpression(node.ModelToScope);
                emitter.StoreLocal(model);
                AddModelScope(x => x.LoadLocal(model));
                EmitNode(node.Block);
                RemoveModelScope();
            }
        }
    }
}