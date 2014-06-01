using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Veil.Parser;

namespace Veil.Handlebars
{
    // TODO:
    // - Stack assertions on #each
    public class HandlebarsParser : ITemplateParser
    {
        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var template = templateReader.ReadToEnd();
            var scopes = new LinkedList<ParserScope>();
            scopes.AddFirst(new ParserScope { Block = SyntaxTreeNode.Block(), ModelInScope = modelType });

            var matcher = new Regex(@"(?<!{)({{[^{}]+}})|({{{[^{}]+}}})(?!})");
            var matches = matcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    scopes.First().Block.Add(SyntaxTreeNode.WriteString(template.Substring(index, match.Index - index)));
                }

                index = match.Index + match.Length;

                var htmlEscape = match.Value.Count(c => c == '{') == 2;
                var token = match.Value.Trim(new[] { '{', '}', ' ', '\t' });

                if (token.StartsWith("#if"))
                {
                    var block = SyntaxTreeNode.Block();
                    var conditional = SyntaxTreeNode.Conditional(HandlebarsExpressionParser.Parse(scopes, token.Substring(4)), block);
                    scopes.First().Block.Add(conditional);
                    scopes.AddFirst(new ParserScope { Block = block, ModelInScope = scopes.First().ModelInScope });
                }
                else if (token == "else")
                {
                    AssertInsideConditionalOnModelBlock(scopes, "{{else}}");
                    scopes.RemoveFirst();
                    var block = SyntaxTreeNode.Block();
                    ((SyntaxTreeNode.ConditionalNode)scopes.First().Block.Nodes.Last()).FalseBlock = block;
                    scopes.AddFirst(new ParserScope { Block = block, ModelInScope = scopes.First().ModelInScope });
                }
                else if (token == "/if")
                {
                    AssertInsideConditionalOnModelBlock(scopes, "{{/if}}");
                    scopes.RemoveFirst();
                }
                else if (token.StartsWith("#each"))
                {
                    var iteration = SyntaxTreeNode.Iterate(
                        HandlebarsExpressionParser.Parse(scopes, token.Substring(6)),
                        SyntaxTreeNode.Block()
                    );
                    scopes.First().Block.Add(iteration);
                    scopes.AddFirst(new ParserScope { Block = iteration.Body, ModelInScope = iteration.ItemType });
                }
                else if (token == "/each")
                {
                    scopes.RemoveFirst();
                }
                else if (token == "#flush")
                {
                    scopes.First().Block.Add(SyntaxTreeNode.Flush());
                }
                else if (token.StartsWith(">"))
                {
                    var partialName = token.Substring(1).Trim();
                    var self = SyntaxTreeNode.ExpressionNode.Self(scopes.First().ModelInScope);
                    scopes.First().Block.Add(SyntaxTreeNode.Include(partialName, self));
                }
                else if (token.StartsWith("!"))
                {
                    // do nothing for comments
                }
                else
                {
                    var expression = HandlebarsExpressionParser.Parse(scopes, token);
                    scopes.First().Block.Add(SyntaxTreeNode.WriteExpression(expression, htmlEscape));
                }
            }
            if (index < template.Length)
            {
                scopes.First().Block.Add(SyntaxTreeNode.WriteString(template.Substring(index)));
            }

            AssertStackOnRootNode(scopes);

            return scopes.First().Block;
        }

        private void AssertStackOnRootNode(LinkedList<ParserScope> scopes)
        {
            if (scopes.Count != 1)
            {
                throw new VeilParserException(String.Format("Mismatched block found. Expected to find the end of the template but found '{0}' open blocks.", scopes.Count));
            }
        }

        private void AssertInsideConditionalOnModelBlock(LinkedList<ParserScope> scopes, string foundToken)
        {
            var faulted = false;
            faulted = scopes.Count < 2;

            if (!faulted)
            {
                faulted = !(scopes.First.Next.Value.Block.Nodes.Last() is SyntaxTreeNode.ConditionalNode);
            }

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a conditional block.", foundToken));
            }
        }

        internal class ParserScope
        {
            public SyntaxTreeNode.BlockNode Block { get; set; }

            public Type ModelInScope { get; set; }
        }
    }
}