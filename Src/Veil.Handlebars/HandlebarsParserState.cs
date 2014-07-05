using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal class HandlebarsParserState
    {
        public HandlebarsBlockStack BlockStack { get; private set; }

        public HandlebarsToken CurrentToken { get; private set; }

        public SyntaxTreeNode ExtendNode { get; set; }

        public bool TrimNextLiteral { get; set; }

        public bool ContinueProcessingToken { get; set; }

        public SyntaxTreeNode RootNode { get { return ExtendNode ?? BlockStack.GetCurrentBlockNode(); } }

        public HandlebarsParserState()
        {
            this.BlockStack = new HandlebarsBlockStack();
        }

        public void WriteLiteral(string s)
        {
            if (TrimNextLiteral)
            {
                s = s.TrimStart();
                TrimNextLiteral = false;
            }
            AddNodeToCurrentBlock(SyntaxTree.WriteString(s));
        }

        public ExpressionNode ParseExpression(string expression)
        {
            return HandlebarsExpressionParser.Parse(BlockStack, expression);
        }

        internal void SetCurrentToken(HandlebarsToken token)
        {
            CurrentToken = token;
            ContinueProcessingToken = false;
        }

        internal SyntaxTreeNode AddNodeToCurrentBlock(SyntaxTreeNode node)
        {
            BlockStack.Peek().Block.Add(node);
            return node;
        }

        internal SyntaxTreeNode LastNode()
        {
            return BlockStack.GetCurrentBlockNode().LastNode();
        }
    }
}