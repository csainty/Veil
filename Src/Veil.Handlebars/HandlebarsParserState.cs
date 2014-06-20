using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal class HandlebarsParserState
    {
        private readonly Stack<string> expressionPrefixes = new Stack<string>();

        public HandlebarsScopeStack Scopes { get; private set; }

        public HandlebarsToken CurrentToken { get; private set; }

        public SyntaxTreeNode ExtendNode { get; set; }

        public bool TrimNextLiteral { get; set; }

        public bool HtmlEscape { get; set; }

        public bool ContinueProcessingToken { get; set; }

        public string TokenText { get; set; }

        public HandlebarsParserState()
        {
            this.Scopes = new HandlebarsScopeStack();
        }

        public void WriteLiteral(string s)
        {
            if (TrimNextLiteral)
            {
                s = s.TrimStart();
                TrimNextLiteral = false;
            }
            Scopes.AddToCurrentScope(SyntaxTree.WriteString(s));
        }

        public ExpressionNode ParseExpression(string expression)
        {
            return HandlebarsExpressionParser.Parse(Scopes, PrefixExpression(expression));
        }

        private string PrefixExpression(string expression)
        {
            if (expressionPrefixes.Count == 0) return expression;
            if (expression == "this") return String.Join(".", expressionPrefixes.Reverse());
            if (expression.StartsWith("../")) return String.Join(".", expressionPrefixes.Skip(1).Reverse().Concat(new[] { expression.Substring(3) }));
            return String.Join(".", expressionPrefixes.Reverse().Concat(new[] { expression }));
        }

        internal void SetCurrentToken(HandlebarsToken token)
        {
            CurrentToken = token;
            TokenText = token.Content.Trim(new[] { '{', '}', ' ', '\t' });
            HtmlEscape = false;
            ContinueProcessingToken = false;
        }

        internal void PushExpressionPrefix(string prefix)
        {
            this.expressionPrefixes.Push(prefix);
        }

        internal void PopExpressionPrefix()
        {
            this.expressionPrefixes.Pop();
        }
    }
}