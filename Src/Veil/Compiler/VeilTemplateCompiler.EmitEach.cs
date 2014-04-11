using System;
using System.Collections.Generic;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitEach<T>(Emit<Action<TextWriter, T>> emitter, EachNode node)
        {
            var itemType = node.Collection.Type.GetEnumerableInterface().GetGenericArguments()[0];
            var enumerable = typeof(IEnumerable<>).MakeGenericType(itemType);
            var getEnumerator = enumerable.GetMethod("GetEnumerator");
            var moveNext = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
            var getCurrent = getEnumerator.ReturnType.GetProperty("Current").GetGetMethod();
            var loop = emitter.DefineLabel();
            var done = emitter.DefineLabel();

            emitter.LoadModelExpressionToStack(node.Collection);
            using (var e = emitter.DeclareLocal(getEnumerator.ReturnType))
            {
                emitter.CallMethod(getEnumerator);
                emitter.StoreLocal(e);

                emitter.MarkLabel(loop);
                emitter.LoadLocal(e);
                emitter.CallMethod(moveNext);
                emitter.BranchIfFalse(done);

                emitter.LoadLocal(e);
                emitter.CallMethod(getCurrent);
                emitter.Pop();
                EmitNode(emitter, node.Body);
                emitter.Branch(loop);

                emitter.MarkLabel(done);
            }
        }
    }
}