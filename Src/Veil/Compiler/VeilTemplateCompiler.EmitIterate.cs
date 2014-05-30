using System;
using System.Collections.Generic;
using System.Reflection;
using Veil.Parser;

namespace Veil.Compiler
{
    // TODO: mimic a using block for the disposable
    internal partial class VeilTemplateCompiler<T>
    {
        private static MethodInfo moveNextMethod = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
        private static MethodInfo disposeMethod = typeof(IDisposable).GetMethod("Dispose");

        private void EmitIterate(SyntaxTreeNode.IterateNode node)
        {
            var enumerable = typeof(IEnumerable<>).MakeGenericType(node.ItemType);
            var getEnumerator = enumerable.GetMethod("GetEnumerator");
            var getCurrent = getEnumerator.ReturnType.GetProperty("Current").GetGetMethod();
            var disposeEnumerator = typeof(IDisposable).IsAssignableFrom(getEnumerator.ReturnType);
            var loop = emitter.DefineLabel();
            var done = emitter.DefineLabel();

            EvaluateExpression(node.Collection);

            if (node.Collection.ResultType == typeof(object))
            {
                emitter.CastClass(enumerable);
            }

            using (var item = emitter.DeclareLocal(node.ItemType))
            using (var en = emitter.DeclareLocal(getEnumerator.ReturnType))
            {
                emitter.CallMethod(getEnumerator);
                emitter.StoreLocal(en);

                emitter.MarkLabel(loop);
                emitter.LoadLocal(en);
                emitter.CallMethod(moveNextMethod);
                emitter.BranchIfFalse(done);

                emitter.LoadLocal(en);
                emitter.CallMethod(getCurrent);
                emitter.StoreLocal(item);

                AddModelScope(e => e.LoadLocal(item));
                EmitNode(node.Body);
                RemoveModelScope();

                emitter.Branch(loop);

                emitter.MarkLabel(done);

                if (disposeEnumerator)
                {
                    emitter.LoadLocal(en);
                    emitter.CallMethod(disposeMethod);
                }
            }
        }
    }
}