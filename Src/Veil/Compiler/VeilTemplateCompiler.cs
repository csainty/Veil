using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private LinkedList<Action<Emit<Action<TextWriter, T>>>> scopeStack;
        private readonly Emit<Action<TextWriter, T>> emitter;
        private readonly Func<string, Type, SyntaxTreeNode> includeParser;
        private readonly IDictionary<string, SyntaxTreeNode> overrideSections;

        public VeilTemplateCompiler(Func<string, Type, SyntaxTreeNode> includeParser)
        {
            scopeStack = new LinkedList<Action<Emit<Action<TextWriter, T>>>>();
            emitter = Emit<Action<TextWriter, T>>.NewDynamicMethod();
            overrideSections = new Dictionary<string, SyntaxTreeNode>();
            this.includeParser = includeParser;
        }

        public Action<TextWriter, T> Compile(SyntaxTreeNode templateSyntaxTree)
        {
            while (templateSyntaxTree is ExtendTemplateNode)
            {
                templateSyntaxTree = Extend((ExtendTemplateNode)templateSyntaxTree);
            }

            AddModelScope(e => e.LoadArgument(1));
            EmitNode(templateSyntaxTree);

            emitter.Return();
            return emitter.CreateDelegate();
        }

        private SyntaxTreeNode Extend(ExtendTemplateNode extendNode)
        {
            foreach (var o in extendNode.Overrides)
            {
                if (overrideSections.ContainsKey(o.Key)) continue;

                overrideSections.Add(o.Key, o.Value);
            }
            return includeParser(extendNode.TemplateName, typeof(T));
        }

        private IDisposable CreateLocalScopeStack()
        {
            var oldScopeStack = scopeStack;
            scopeStack = new LinkedList<Action<Emit<Action<TextWriter, T>>>>();
            return new ActionDisposable(() =>
            {
                scopeStack = oldScopeStack;
            });
        }

        private void AddModelScope(Action<Emit<Action<TextWriter, T>>> scope)
        {
            scopeStack.AddFirst(scope);
        }

        private void RemoveModelScope()
        {
            scopeStack.RemoveFirst();
        }

        private void PushCurrentModelOnStack()
        {
            scopeStack.First.Value.Invoke(emitter);
        }

        private void EvaluateExpression(ExpressionNode expression)
        {
            switch (expression.Scope)
            {
                case ExpressionScope.CurrentModelOnStack:
                    scopeStack.First.Value.Invoke(emitter);
                    break;

                case ExpressionScope.RootModel:
                    scopeStack.Last.Value.Invoke(emitter);
                    break;

                case ExpressionScope.ModelOfParentScope:
                    scopeStack.First.Next.Value.Invoke(emitter);
                    break;

                default:
                    throw new VeilCompilerException("Unknown expression scope '{0}'".FormatInvariant(expression.Scope));
            }
            EvaluateExpressionAgainstModelOnStack(expression);
        }

        private void EvaluateExpressionAgainstModelOnStack(ExpressionNode expression)
        {
            if (expression is PropertyExpressionNode)
            {
                emitter.CallMethod(((PropertyExpressionNode)expression).PropertyInfo.GetGetMethod());
            }
            else if (expression is FieldExpressionNode)
            {
                emitter.LoadField(((FieldExpressionNode)expression).FieldInfo);
            }
            else if (expression is SubModelExpressionNode)
            {
                EvaluateExpressionAgainstModelOnStack(((SubModelExpressionNode)expression).ModelExpression);
                EvaluateExpressionAgainstModelOnStack(((SubModelExpressionNode)expression).SubModelExpression);
            }
            else if (expression is FunctionCallExpressionNode)
            {
                emitter.CallMethod(((FunctionCallExpressionNode)expression).MethodInfo);
            }
            else if (expression is CollectionHasItemsExpressionNode)
            {
                var hasItems = (CollectionHasItemsExpressionNode)expression;
                var count = hasItems.CollectionExpression.ResultType.GetCollectionInterface().GetProperty("Count");
                EvaluateExpressionAgainstModelOnStack(hasItems.CollectionExpression);
                emitter.CallMethod(count.GetGetMethod());
                emitter.LoadConstant(0);
                emitter.CompareEqual();
                emitter.LoadConstant(0);
                emitter.CompareEqual();
            }
            else if (expression is LateBoundExpressionNode)
            {
                emitter.LoadConstant(((LateBoundExpressionNode)expression).ItemName);
                emitter.CallMethod(runtimeBindMethod);
            }
            else if (expression is SelfExpressionNode)
            {
            }
            else
            {
                throw new VeilCompilerException("Unknown expression type '{0}'".FormatInvariant(expression.GetType().Name));
            }
        }

        private void LoadWriterToStack()
        {
            emitter.LoadArgument(0);
        }

        private void CallWriteFor(Type typeOfItemOnStack)
        {
            if (!writers.ContainsKey(typeOfItemOnStack)) throw new VeilCompilerException("Unable to call TextWriter.Write() for item of type '{0}'".FormatInvariant(typeOfItemOnStack.Name));
            emitter.CallMethod(writers[typeOfItemOnStack]);
        }

        private static readonly MethodInfo runtimeBindMethod = typeof(Helpers).GetMethod("RuntimeBind");

        private static readonly IDictionary<Type, MethodInfo> writers = new Dictionary<Type, MethodInfo>
        {
            { typeof(string), typeof(TextWriter).GetMethod("Write", new[] { typeof(string) }) },
            { typeof(int), typeof(TextWriter).GetMethod("Write", new[] { typeof(int) }) },
            { typeof(double), typeof(TextWriter).GetMethod("Write", new[] { typeof(double) }) },
            { typeof(float), typeof(TextWriter).GetMethod("Write", new[] { typeof(float) }) },
            { typeof(long), typeof(TextWriter).GetMethod("Write", new[] { typeof(long) }) },
            { typeof(uint), typeof(TextWriter).GetMethod("Write", new[] { typeof(uint) }) },
            { typeof(ulong), typeof(TextWriter).GetMethod("Write", new[] { typeof(ulong) }) },
        };
    }
}