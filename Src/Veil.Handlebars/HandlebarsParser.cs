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
            var blockStack = new Stack<ParserScope>();
            blockStack.Push(new ParserScope { Block = SyntaxTreeNode.Block(), ModelInScope = modelType });

            var matcher = new Regex(@"(?<!{)({{[^{}]+}})|({{{[^{}]+}}})(?!})");
            var matches = matcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    blockStack.Peek().Block.Add(SyntaxTreeNode.WriteString(template.Substring(index, match.Index - index)));
                }

                index = match.Index + match.Length;

                var htmlEscape = match.Value.Count(c => c == '{') == 2;
                var token = match.Value.Trim(new[] { '{', '}', ' ', '\t' });

                if (token.StartsWith("#if"))
                {
                    var block = SyntaxTreeNode.Block();
                    var conditional = SyntaxTreeNode.Conditional(HandlebarsExpressionParser.Parse(blockStack.Peek().ModelInScope, token.Substring(4)), block);
                    blockStack.Peek().Block.Add(conditional);
                    blockStack.Push(new ParserScope { Block = block, ModelInScope = blockStack.Peek().ModelInScope });
                }
                else if (token == "else")
                {
                    AssertInsideConditionalOnModelBlock(blockStack, "{{else}}");
                    blockStack.Pop();
                    var block = SyntaxTreeNode.Block();
                    ((SyntaxTreeNode.ConditionalNode)blockStack.Peek().Block.Nodes.Last()).FalseBlock = block;
                    blockStack.Push(new ParserScope { Block = block, ModelInScope = blockStack.Peek().ModelInScope });
                }
                else if (token == "/if")
                {
                    AssertInsideConditionalOnModelBlock(blockStack, "{{/if}}");
                    blockStack.Pop();
                }
                else if (token.StartsWith("#each"))
                {
                    var iteration = SyntaxTreeNode.Iterate(
                        HandlebarsExpressionParser.Parse(blockStack.Peek().ModelInScope, token.Substring(6)),
                        SyntaxTreeNode.Block()
                    );
                    blockStack.Peek().Block.Add(iteration);
                    blockStack.Push(new ParserScope { Block = iteration.Body, ModelInScope = iteration.ItemType });
                }
                else if (token == "/each")
                {
                    blockStack.Pop();
                }
                else if (token == "#flush")
                {
                    blockStack.Peek().Block.Add(SyntaxTreeNode.Flush());
                }
                else
                {
                    var expression = HandlebarsExpressionParser.Parse(blockStack.Peek().ModelInScope, token);
                    blockStack.Peek().Block.Add(SyntaxTreeNode.WriteExpression(expression, htmlEscape && expression.ResultType == typeof(string)));
                }
            }
            if (index < template.Length)
            {
                blockStack.Peek().Block.Add(SyntaxTreeNode.WriteString(template.Substring(index)));
            }

            AssertStackOnRootNode(blockStack);

            return blockStack.Pop().Block;
        }

        private void AssertStackOnRootNode(Stack<ParserScope> scopes)
        {
            if (scopes.Count != 1)
            {
                throw new VeilParserException(String.Format("Mismatched block found. Expected to find the end of the template but found '{0}' open blocks.", scopes.Count));
            }
        }

        private void AssertInsideConditionalOnModelBlock(Stack<ParserScope> scopes, string foundToken)
        {
            var faulted = false;
            faulted = scopes.Count < 2;

            if (!faulted)
            {
                var block = scopes.Pop();
                faulted = !(scopes.Peek().Block.Nodes.Last() is SyntaxTreeNode.ConditionalNode);
                scopes.Push(block);
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