using System.Collections.Generic;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitIterate<T>(VeilCompilerState<T> state, SyntaxTreeNode.IterateNode node)
        {
            var enumerable = typeof(IEnumerable<>).MakeGenericType(node.ItemType);
            var getEnumerator = enumerable.GetMethod("GetEnumerator");
            var moveNext = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
            var getCurrent = getEnumerator.ReturnType.GetProperty("Current").GetGetMethod();
            var loop = state.Emitter.DefineLabel();
            var done = state.Emitter.DefineLabel();

            state.PushExpressionScopeOnStack(node.Collection);
            state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Collection);
            using (var item = state.Emitter.DeclareLocal(node.ItemType))
            using (var en = state.Emitter.DeclareLocal(getEnumerator.ReturnType))
            {
                state.Emitter.CallMethod(getEnumerator);
                state.Emitter.StoreLocal(en);

                state.Emitter.MarkLabel(loop);
                state.Emitter.LoadLocal(en);
                state.Emitter.CallMethod(moveNext);
                state.Emitter.BranchIfFalse(done);

                state.Emitter.LoadLocal(en);
                state.Emitter.CallMethod(getCurrent);
                state.Emitter.StoreLocal(item);

                state.AddModelScope(e => e.LoadLocal(item));
                EmitNode(state, node.Body);
                state.RemoveModelScope();

                state.Emitter.Branch(loop);

                state.Emitter.MarkLabel(done);
            }
        }
    }
}