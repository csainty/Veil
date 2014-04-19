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
            var scopeStack = new LinkedList<ParserScope>();
            scopeStack.AddFirst(new ParserScope { Block = SyntaxTreeNode.Block(), ModelType = modelType });

            var matcher = new Regex(@"@[A-Za-z0-9\.]*;?");
            var matches = matcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    scopeStack.First.Value.Block.Add(SyntaxTreeNode.WriteString(template.Substring(index, match.Index - index)));
                }
                index = match.Index + match.Length;

                var token = match.Value.Trim(new[] { '@', ';' });
                if (token.StartsWith("Each."))
                {
                    token = token.Substring(5);
                    var each = SyntaxTreeNode.Iterate(
                        SuperSimpleExpressionParser.Parse(scopeStack, token),
                        SyntaxTreeNode.Block()
                    );
                    if (!each.CollectionIsValid) throw new VeilParserException(String.Format("The expression '{0}' does not evaluate to a valid collection type", token));
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new ParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (token == "EndEach")
                {
                    scopeStack.RemoveFirst();
                }
                else
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack, token);
                    scopeStack.First.Value.Block.Add(SyntaxTreeNode.WriteExpression(expression));
                }
            }
            if (index < template.Length)
            {
                scopeStack.First.Value.Block.Add(SyntaxTreeNode.WriteString(template.Substring(index)));
            }

            return scopeStack.First.Value.Block;
        }

        internal class ParserScope
        {
            public SyntaxTreeNode.BlockNode Block { get; set; }

            public Type ModelType { get; set; }
        }
    }
}