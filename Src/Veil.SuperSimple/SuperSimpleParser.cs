using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Veil.SuperSimple
{
    public class SuperSimpleParser : ITemplateParser
    {
        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var template = templateReader.ReadToEnd();
            var scopeStack = new Stack<ParserScope>();
            scopeStack.Push(new ParserScope { Block = SyntaxTreeNode.Block(), ModelType = modelType });

            var matcher = new Regex(@"@[A-Za-z0-9\.]*;?");
            var matches = matcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    scopeStack.Peek().Block.Add(SyntaxTreeNode.StringLiteral(template.Substring(index, match.Index - index)));
                }
                index = match.Index + match.Length;

                var token = match.Value.Trim(new[] { '@', ';' });
                if (token.StartsWith("Each."))
                {
                    token = token.Substring(5);
                    var each = SyntaxTreeNode.Each(
                        SuperSimpleExpressionParser.Parse(modelType, token),
                        SyntaxTreeNode.Block()
                    );
                    scopeStack.Peek().Block.Add(each);
                    scopeStack.Push(new ParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (token == "EndEach")
                {
                    scopeStack.Pop();
                }
                else
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack.Peek().ModelType, token);
                    if (expression == null)
                    {
                        scopeStack.Peek().Block.Add(SyntaxTreeNode.StringLiteral("[ERR!]"));
                    }
                    else
                    {
                        scopeStack.Peek().Block.Add(SyntaxTreeNode.Expression(expression));
                    }
                }
            }
            if (index < template.Length)
            {
                scopeStack.Peek().Block.Add(SyntaxTreeNode.StringLiteral(template.Substring(index)));
            }

            return scopeStack.Peek().Block;
        }

        private class ParserScope
        {
            public SyntaxTreeNode.BlockNode Block { get; set; }

            public Type ModelType { get; set; }
        }
    }
}