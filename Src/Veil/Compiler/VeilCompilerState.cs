using System;
using System.Collections.Generic;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal class VeilCompilerState<T>
    {
        private LinkedList<Action<Emit<Action<TextWriter, T>>>> scopeStack = new LinkedList<Action<Emit<Action<TextWriter, T>>>>();

        private Emit<Action<TextWriter, T>> emitter = Emit<Action<TextWriter, T>>.NewDynamicMethod();

        public Emit<Action<TextWriter, T>> Emitter { get { return this.emitter; } }

        public VeilCompilerState()
        {
            AddRootModelScope();
        }

        private void AddRootModelScope()
        {
            this.AddModelScope(e => e.LoadArgument(1));
        }

        internal void AddModelScope(Action<Emit<Action<TextWriter, T>>> scope)
        {
            this.scopeStack.AddFirst(scope);
        }

        internal void RemoveModelScope()
        {
            if (this.scopeStack.Count == 1)
            {
                throw new VeilCompilerException("Tried to remove the last model scope. Something has gone wrong");
            }
            this.scopeStack.RemoveFirst();
        }

        internal void PushCurrentModelOnStack()
        {
            this.scopeStack.First.Value.Invoke(this.emitter);
        }

        internal void PushExpressionScopeOnStack(SyntaxTreeNode.ExpressionNode node)
        {
            switch (node.Scope)
            {
                case SyntaxTreeNode.ExpressionScope.CurrentModelOnStack:
                    this.scopeStack.First.Value.Invoke(this.emitter);
                    break;

                case SyntaxTreeNode.ExpressionScope.RootModel:
                    this.scopeStack.Last.Value.Invoke(this.emitter);
                    break;

                default:
                    throw new VeilCompilerException("Uknown expression scope '{0}'".FormatInvariant(node.Scope));
            }
        }
    }
}