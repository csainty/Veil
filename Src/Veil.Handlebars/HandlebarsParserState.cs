using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal class HandlebarsParserState
    {
        public Stack<string> ExpressionPrefixes { get; private set; }

        public HandlebarsScopeStack Scopes { get; private set; }

        public HandlebarsToken CurrentToken { get; private set; }

        public SyntaxTreeNode ExtendNode { get; set; }

        public bool TrimNextLiteral { get; set; }

        public bool HtmlEscape { get; set; }

        public bool ContinueProcessingToken { get; set; }

        public string TokenText { get; set; }

        public SyntaxTreeNode RootNode { get { return ExtendNode ?? Scopes.GetCurrentBlock(); } }

        public HandlebarsParserState()
        {
            this.Scopes = new HandlebarsScopeStack();
            this.ExpressionPrefixes = new Stack<string>();
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
            if (ExpressionPrefixes.Count == 0) return expression;
            if (expression == "this") return String.Join(".", ExpressionPrefixes.Reverse());
            if (expression.StartsWith("../")) return String.Join(".", ExpressionPrefixes.Skip(1).Reverse().Concat(new[] { expression.Substring(3) }));
            return String.Join(".", ExpressionPrefixes.Reverse().Concat(new[] { expression }));
        }

        internal void SetCurrentToken(HandlebarsToken token)
        {
            CurrentToken = token;
            TokenText = token.Content.Trim(new[] { '{', '}', ' ', '\t' });
            HtmlEscape = false;
            ContinueProcessingToken = false;
        }
    }
}