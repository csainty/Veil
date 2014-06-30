using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.SuperSimple
{
    internal class SuperSimpleTemplateParserState
    {
        private readonly LinkedList<SuperSimpleTemplateParserScope> scopeStack = new LinkedList<SuperSimpleTemplateParserScope>();

        public SuperSimpleToken CurrentToken { get; set; }

        public BlockNode CurrentBlock { get { return this.scopeStack.First().Block; } }

        public void PushNewScope(Type modelType)
        {
            this.PushNewScope(SyntaxTree.Block(), modelType);
        }

        public void PushNewScope(BlockNode blockNode)
        {
            this.PushNewScope(blockNode, this.CurrentTypeInScope());
        }

        public void PushNewScope(BlockNode blockNode, Type modelType)
        {
            this.scopeStack.AddFirst(new SuperSimpleTemplateParserScope
            {
                Block = blockNode,
                ModelType = modelType
            });
        }

        public void AddNodeToCurrentBlock(SyntaxTreeNode node)
        {
            this.scopeStack.First().Block.Add(node);
        }

        public Type CurrentTypeInScope()
        {
            return this.scopeStack.First().ModelType;
        }

        public ExpressionNode ParseCurrentTokenExpression()
        {
            var expression = this.CurrentToken.Content.Substring(this.CurrentToken.Content.IndexOf('.') + 1);
            return ParseExpression(expression);
        }

        public ExpressionNode ParseExpression(string expression)
        {
            return SuperSimpleExpressionParser.Parse(this.scopeStack, expression);
        }

        public void PopCurrentScope()
        {
            this.scopeStack.RemoveFirst();
        }

        public SuperSimpleNameModel ParseCurrentTokenNameAndModelExpression()
        {
            return SuperSimpleNameModelParser.Parse(this.CurrentToken.Content);
        }
    }
}