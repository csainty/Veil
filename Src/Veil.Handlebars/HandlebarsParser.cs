using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    /// <summary>
    /// Veil parser for the Handlebars syntax
    /// </summary>
    public class HandlebarsParser : ITemplateParser
    {
        // TODO: A serious refactor / rewrite is needed for this class
        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var tokens = HandlebarsTokenizer.Tokenize(templateReader);
            var scopes = new LinkedList<ParserScope>();
            SyntaxTreeNode extendNode = null;
            scopes.AddFirst(new ParserScope { Block = SyntaxTree.Block(), ModelInScope = modelType });
            var trimNextLiteral = false;
            var expressionPrefixes = new Stack<string>();

            Action<String> writeLiteral = s =>
            {
                if (trimNextLiteral)
                {
                    s = s.TrimStart();
                    trimNextLiteral = false;
                }
                scopes.First().Block.Add(SyntaxTree.WriteString(s));
            };
            Func<string, string> prefixExpression = s =>
            {
                if (expressionPrefixes.Count == 0) return s;
                if (s == "this") return String.Join(".", expressionPrefixes.Reverse());
                if (s.StartsWith("../")) return String.Join(".", expressionPrefixes.Skip(1).Reverse().Concat(new[] { s.Substring(3) }));
                return String.Join(".", expressionPrefixes.Reverse().Concat(new[] { s }));
            };
            Func<string, ExpressionNode> parseExpression = e => HandlebarsExpressionParser.Parse(scopes, prefixExpression(e));

            foreach (var currentToken in tokens)
            {
                if (!currentToken.IsSyntaxToken)
                {
                    writeLiteral(currentToken.Content);
                    continue;
                }
                var htmlEscape = currentToken.Content.Count(c => c == '{') == 2;
                var token = currentToken.Content.Trim(new[] { '{', '}', ' ', '\t' });

                if (token.StartsWith("~"))
                {
                    TrimLastLiteral(scopes.First().Block);
                }
                trimNextLiteral = token.EndsWith("~");
                token = token.Trim('~', ' ');

                if (token.StartsWith("#if"))
                {
                    var block = SyntaxTree.Block();
                    var conditional = SyntaxTree.Conditional(parseExpression(token.Substring(4)), block);
                    scopes.First().Block.Add(conditional);
                    scopes.AddFirst(new ParserScope { Block = block, ModelInScope = scopes.First().ModelInScope });
                }
                else if (token == "else")
                {
                    if (IsInEachBlock(scopes))
                    {
                        scopes.RemoveFirst();
                        var elseBlock = ((IterateNode)scopes.First().Block.Nodes.Last()).EmptyBody;
                        scopes.AddFirst(new ParserScope { Block = elseBlock, ModelInScope = scopes.First().ModelInScope });
                    }
                    else
                    {
                        AssertInsideConditionalOnModelBlock(scopes, "{{else}}");
                        scopes.RemoveFirst();
                        var block = SyntaxTree.Block();
                        ((ConditionalNode)scopes.First().Block.Nodes.Last()).FalseBlock = block;
                        scopes.AddFirst(new ParserScope { Block = block, ModelInScope = scopes.First().ModelInScope });
                    }
                }
                else if (token == "/if")
                {
                    AssertInsideConditionalOnModelBlock(scopes, "{{/if}}");
                    scopes.RemoveFirst();
                }
                else if (token.StartsWith("#unless"))
                {
                    var block = SyntaxTree.Block();
                    var conditional = SyntaxTree.Conditional(parseExpression(token.Substring(8)), SyntaxTree.Block(), block);
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
                    var iteration = SyntaxTree.Iterate(
                        parseExpression(token.Substring(6)),
                        SyntaxTree.Block()
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
                    scopes.First().Block.Add(SyntaxTree.Flush());
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
                    var self = Expression.Self(scopes.First().ModelInScope);
                    scopes.First().Block.Add(SyntaxTree.Include(partialName, self));
                }
                else if (token.StartsWith("<"))
                {
                    AssertSyntaxTreeIsEmpty(scopes);
                    var masterName = token.Substring(1).Trim();
                    extendNode = SyntaxTree.Extend(masterName, new Dictionary<string, SyntaxTreeNode>
                    {
                        {"body", scopes.First().Block}
                    });
                }
                else if (token == "body")
                {
                    scopes.First().Block.Add(SyntaxTree.Override("body"));
                }
                else if (token.StartsWith("!"))
                {
                    // do nothing for comments
                }
                else
                {
                    var expression = parseExpression(token);
                    scopes.First().Block.Add(SyntaxTree.WriteExpression(expression, htmlEscape));
                }
            }

            AssertStackOnRootNode(scopes);

            return extendNode ?? scopes.First().Block;
        }

        private static void TrimLastLiteral(BlockNode blockNode)
        {
            var literal = blockNode.Nodes.Last() as WriteLiteralNode;
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
                faulted = !(scopes.First.Next.Value.Block.Nodes.Last() is ConditionalNode);
            }

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a conditional block.", foundToken));
            }
        }

        private static void AssertSyntaxTreeIsEmpty(LinkedList<ParserScope> scopes)
        {
            if (scopes.Count > 1 || !scopes.First().Block.IsEmpty())
            {
                throw new VeilParserException("There can be no content before a {{< }} expression.");
            }
        }

        private static bool IsInEachBlock(LinkedList<ParserScope> scopes)
        {
            if (scopes.Count < 2)
            {
                return false;
            }

            return scopes.First.Next.Value.Block.Nodes.Last() is IterateNode;
        }

        internal class ParserScope
        {
            public BlockNode Block { get; set; }

            public Type ModelInScope { get; set; }
        }
    }
}