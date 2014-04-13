using System;
using System.Collections.Generic;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitEach<T>(VeilCompilerState<T> state, EachNode node)
        {
            var itemType = node.Collection.Type.GetEnumerableInterface().GetGenericArguments()[0];
            var enumerable = typeof(IEnumerable<>).MakeGenericType(itemType);
            var getEnumerator = enumerable.GetMethod("GetEnumerator");
            var moveNext = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
            var getCurrent = getEnumerator.ReturnType.GetProperty("Current").GetGetMethod();
            var loop = state.Emitter.DefineLabel();
            var done = state.Emitter.DefineLabel();

            state.PushCurrentModelOnStack();
            state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Collection);
            using (var item = state.Emitter.DeclareLocal(itemType))
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