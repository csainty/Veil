using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    internal class HandlebarsBlockStack
    {
        private readonly LinkedList<HandlebarsParserBlock> blocks = new LinkedList<HandlebarsParserBlock>();

        public void PushBlock(HandlebarsParserBlock block)
        {
            blocks.AddFirst(block);
        }

        public int Count { get { return blocks.Count; } }

        public void PushNewBlockWithModel(Type modelInScope)
        {
            PushBlock(new HandlebarsParserBlock
            {
                Block = SyntaxTree.Block(),
                ModelInScope = modelInScope
            });
        }

        public void PushModelInheritingBlock(BlockNode block)
        {
            PushBlock(new HandlebarsParserBlock
            {
                Block = block,
                ModelInScope = GetCurrentModelType()
            });
        }

        public HandlebarsParserBlock PopBlock()
        {
            var block = blocks.First.Value;
            blocks.RemoveFirst();
            return block;
        }

        public HandlebarsParserBlock Peek()
        {
            return blocks.First.Value;
        }

        public BlockNode GetCurrentBlockNode()
        {
            return Peek().Block;
        }

        public Type GetCurrentModelType()
        {
            return Peek().ModelInScope;
        }

        public Type GetParentModelType()
        {
            return blocks.First.Next.Value.ModelInScope;
        }

        public T GetCurrentBlockContainer<T>() where T : SyntaxTreeNode
        {
            return (T)blocks.First.Next.Value.Block.Nodes.Last();
        }

        public bool IsCurrentBlockContainerOfType<T>() where T : SyntaxTreeNode
        {
            return blocks.First.Next.Value.Block.Nodes.Last() is T;
        }
    }
}