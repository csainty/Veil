using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    internal class HandlebarsScopeStack
    {
        private readonly LinkedList<HandlebarsParserScope> scopes = new LinkedList<HandlebarsParserScope>();

        public void PushScope(HandlebarsParserScope scope)
        {
            scopes.AddFirst(scope);
        }

        public int Count { get { return scopes.Count; } }

        public void PushScope(Type modelInScope)
        {
            PushScope(new HandlebarsParserScope
            {
                Block = SyntaxTree.Block(),
                ModelInScope = modelInScope
            });
        }

        public void PushInheritedScope(BlockNode block)
        {
            PushScope(new HandlebarsParserScope
            {
                Block = block,
                ModelInScope = GetTypeOfModelInScope()
            });
        }

        public HandlebarsParserScope PopScope()
        {
            var scope = scopes.First.Value;
            scopes.RemoveFirst();
            return scope;
        }

        public HandlebarsParserScope Peek()
        {
            return scopes.First.Value;
        }

        public BlockNode GetCurrentBlock()
        {
            return Peek().Block;
        }

        public Type GetTypeOfModelInScope()
        {
            return Peek().ModelInScope;
        }

        public Type GetTypeOfParentScopeModel()
        {
            return scopes.First.Next.Value.ModelInScope;
        }

        public T GetCurrentScopeContainer<T>() where T : SyntaxTreeNode
        {
            return (T)scopes.First.Next.Value.Block.Nodes.Last();
        }

        public SyntaxTreeNode AddToCurrentScope(SyntaxTreeNode node)
        {
            Peek().Block.Add(node);
            return node;
        }
    }
}