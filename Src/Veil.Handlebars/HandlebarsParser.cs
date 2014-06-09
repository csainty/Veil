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
            var trimNextLiteral = false;
            var expressionPrefixes = new Stack<string>();

            Action<String> writeLiteral = s =>
            {
                if (trimNextLiteral)
                {
                    s = s.TrimStart();
                    trimNextLiteral = false;
                }
                scopes.First().Block.Add(SyntaxTreeNode.WriteString(s));
            };
            Func<string, string> prefixExpression = s =>
            {
                if (expressionPrefixes.Count == 0) return s;
                if (s == "this") return String.Join(".", expressionPrefixes.Reverse());
                if (s.StartsWith("../")) return String.Join(".", expressionPrefixes.Skip(1).Reverse().Concat(new[] { s.Substring(3) }));
                return String.Join(".", expressionPrefixes.Reverse().Concat(new[] { s }));
            };
            Func<string, SyntaxTreeNode.ExpressionNode> parseExpression = e => HandlebarsExpressionParser.Parse(scopes, prefixExpression(e));

            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    writeLiteral(template.Substring(index, match.Index - index));
                }

                index = match.Index + match.Length;

                var htmlEscape = match.Value.Count(c => c == '{') == 2;
                var token = match.Value.Trim(new[] { '{', '}', ' ', '\t' });

                if (token.StartsWith("~"))
                {
                    TrimLastLiteral(scopes.First().Block);
                }
                trimNextLiteral = token.EndsWith("~");
                token = token.Trim('~', ' ');

                if (token.StartsWith("#if"))
                {
                    var block = SyntaxTreeNode.Block();
                    var conditional = SyntaxTreeNode.Conditional(parseExpression(token.Substring(4)), block);
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
                else if (token.StartsWith("#unless"))
                {
                    var block = SyntaxTreeNode.Block();
                    var conditional = SyntaxTreeNode.Conditional(parseExpression(token.Substring(8)), SyntaxTreeNode.Block(), block);
                    scopes.First().Block.Add(conditional);
                    scopes.AddFirst(new ParserScope { Block = block, ModelInScope = scopes.First().ModelInScope });
                }
                else if (token == "/unless")
                {
                    AssertInsideConditionalOnModelBlock(scopes, "{{/unless}}");
                    scopes.RemoveFirst();
                }
                else if (token.StartsWith("#each"))
                {
                    var iteration = SyntaxTreeNode.Iterate(
                        parseExpression(token.Substring(6)),
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
                else if (token.StartsWith("#with"))
                {
                    expressionPrefixes.Push(token.Substring(6).Trim());
                }
                else if (token == "/with")
                {
                    expressionPrefixes.Pop();
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
                    var expression = parseExpression(token);
                    scopes.First().Block.Add(SyntaxTreeNode.WriteExpression(expression, htmlEscape));
                }
            }
            if (index < template.Length)
            {
                writeLiteral(template.Substring(index));
            }

            AssertStackOnRootNode(scopes);

            return scopes.First().Block;
        }

        private static void TrimLastLiteral(SyntaxTreeNode.BlockNode blockNode)
        {
            var literal = blockNode.Nodes.Last() as SyntaxTreeNode.WriteLiteralNode;
            if (literal == null) return;
            literal.LiteralContent = literal.LiteralContent.TrimEnd();
        }

        private static void AssertStackOnRootNode(LinkedList<ParserScope> scopes)
        {
            if (scopes.Count != 1)
            {
                throw new VeilParserException(String.Format("Mismatched block found. Expected to find the end of the template but found '{0}' open blocks.", scopes.Count));
            }
        }

        private static void AssertInsideConditionalOnModelBlock(LinkedList<ParserScope> scopes, string foundToken)
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