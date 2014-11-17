using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static MethodInfo moveNextMethod = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
        private static MethodInfo disposeMethod = typeof(IDisposable).GetMethod("Dispose");

        private Expression Iterate(IterateNode node)
        {
            var enumerable = typeof(IEnumerable<>).MakeGenericType(node.ItemType);
            var getEnumerator = enumerable.GetMethod("GetEnumerator");
            var getCurrent = getEnumerator.ReturnType.GetProperty("Current").GetGetMethod();
            var disposeEnumerator = typeof(IDisposable).IsAssignableFrom(getEnumerator.ReturnType);

            var currentElement = Expression.Variable(node.ItemType, "current");
            var isMoreElements = Expression.Variable(typeof(bool), "areMoreElements");
            var hasElements = Expression.Variable(typeof(bool), "hasElements");
            var enumerator = Expression.Variable(getEnumerator.ReturnType, "enumerator");
            var exit = Expression.Label();
            var collection = ParseExpression(node.Collection);

            var x = collection;
            if (collection.Type == typeof(object))
            {
                x = Expression.Convert(x, enumerable);
            }

            this.PushScope(currentElement);
            var loopBody = Node(node.Body);
            this.PopScope();

            var result = Expression.Block(
                new[] { enumerator, hasElements },
                Expression.Assign(hasElements, Expression.Constant(false)),
                Expression.Assign(enumerator, Expression.Call(x, getEnumerator)),
                Expression.Loop(Expression.Block(
                    new[] { isMoreElements },
                    Expression.Assign(isMoreElements, Expression.Call(enumerator, moveNextMethod)),
                    Expression.IfThenElse(Expression.IsFalse(isMoreElements),
                        Expression.Break(exit),
                        Expression.Block(
                            new[] { currentElement },
                            Expression.Assign(currentElement, Expression.Property(enumerator, getCurrent)),
                            Expression.Assign(hasElements, Expression.Constant(true)),
                            loopBody
                        )
                    )
                ), exit),
                Expression.IfThen(Expression.IsFalse(hasElements), Node(node.EmptyBody))
            );
            return result;
        }
    }
}