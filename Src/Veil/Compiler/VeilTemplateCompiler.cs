using System;
using System.Collections.Generic;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private readonly LinkedList<Action<Emit<Action<TextWriter, T>>>> scopeStack;
        private readonly Emit<Action<TextWriter, T>> emitter;

        public VeilTemplateCompiler()
        {
            scopeStack = new LinkedList<Action<Emit<Action<TextWriter, T>>>>();
            emitter = Emit<Action<TextWriter, T>>.NewDynamicMethod();
        }

        public Action<TextWriter, T> Compile(SyntaxTreeNode templateSyntaxTree)
        {
            AddModelScope(e => e.LoadArgument(1));
            EmitNode(templateSyntaxTree);

            emitter.Return();
            return emitter.CreateDelegate();
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

        private void PushExpressionScopeOnStack(SyntaxTreeNode.ExpressionNode node)
        {
            switch (node.Scope)
            {
                case SyntaxTreeNode.ExpressionScope.CurrentModelOnStack:
                    scopeStack.First.Value.Invoke(emitter);
                    break;

                case SyntaxTreeNode.ExpressionScope.RootModel:
                    scopeStack.Last.Value.Invoke(emitter);
                    break;

                default:
                    throw new VeilCompilerException("Uknown expression scope '{0}'".FormatInvariant(node.Scope));
            }
        }
    }
}