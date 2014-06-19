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
            var allDone = emitter.DefineLabel();
            var empty = emitter.DefineLabel();
            if (node.Collection.ResultType.IsArray)
            {
                var done = emitter.DefineLabel();
                var loop = emitter.DefineLabel();
                using (var index = emitter.DeclareLocal(typeof(int)))
                using (var length = emitter.DeclareLocal(typeof(int)))
                using (var item = emitter.DeclareLocal(node.ItemType))
                {
                    EvaluateExpression(node.Collection);
                    emitter.LoadLength(node.ItemType);
                    emitter.StoreLocal(length);

                    emitter.LoadLocal(length);
                    emitter.BranchIfFalse(empty);

                    emitter.LoadConstant(0);
                    emitter.StoreLocal(index);

                    emitter.MarkLabel(loop);
                    emitter.LoadLocal(length);
                    emitter.LoadLocal(index);
                    emitter.CompareEqual();
                    emitter.BranchIfTrue(done);

                    EvaluateExpression(node.Collection);
                    emitter.LoadLocal(index);
                    emitter.LoadElement(node.ItemType);
                    emitter.StoreLocal(item);

                    AddModelScope(e => e.LoadLocal(item));
                    EmitNode(node.Body);
                    RemoveModelScope();

                    emitter.LoadLocal(index);
                    emitter.LoadConstant(1);
                    emitter.Add();
                    emitter.StoreLocal(index);
                    emitter.Branch(loop);

                    emitter.MarkLabel(done);
                    emitter.Branch(allDone);
                }
            }
            else
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
                using (var hasItems = emitter.DeclareLocal(typeof(bool)))
                using (var en = emitter.DeclareLocal(getEnumerator.ReturnType))
                {
                    emitter.CallMethod(getEnumerator);
                    emitter.StoreLocal(en);

                    emitter.MarkLabel(loop);
                    emitter.LoadLocal(en);
                    emitter.CallMethod(moveNextMethod);
                    emitter.BranchIfFalse(done);

                    emitter.LoadConstant(1);
                    emitter.StoreLocal(hasItems);
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

                    emitter.LoadLocal(hasItems);
                    emitter.BranchIfTrue(allDone);
                }
            }
            emitter.MarkLabel(empty);
            EmitNode(node.EmptyBody);
            emitter.MarkLabel(allDone);
        }
    }
}