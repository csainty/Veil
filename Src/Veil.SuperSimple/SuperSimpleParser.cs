using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.SuperSimple
{
    /// <summary>
    /// A Veil parser for the SuperSimple syntax
    /// </summary>
    public class SuperSimpleParser : ITemplateParser
    {
        private static Regex NameMatcher = new Regex(@".*?\[\'(?<Name>.*?)\'(,(?<Model>.*))?\]", RegexOptions.Compiled);

        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var tokens = SuperSimpleTokenizer.Tokenize(templateReader).ToArray();
            var firstToken = tokens.First();
            if (firstToken.IsSyntaxToken && firstToken.Content.StartsWith("@Master"))
            {
                return ParseMaster(tokens, modelType);
            }

            return ParseTemplate(tokens, modelType);
        }

        private SyntaxTreeNode ParseTemplate(IEnumerable<SuperSimpleToken> tokens, Type modelType)
        {
            var scopeStack = new LinkedList<ParserScope>();
            scopeStack.AddFirst(new ParserScope { Block = SyntaxTree.Block(), ModelType = modelType });

            foreach (var token in tokens)
            {
                if (!token.IsSyntaxToken)
                {
                    scopeStack.First().Block.Add(SyntaxTree.WriteString(token.Content));
                    continue;
                }

                var currentToken = token.Content.Trim(new[] { '@', ';' });
                if (currentToken == "Each")
                {
                    var each = SyntaxTree.Iterate(
                        Expression.Self(scopeStack.First.Value.ModelType),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new ParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (currentToken.StartsWith("Each"))
                {
                    currentToken = currentToken.Substring(5);
                    var each = SyntaxTree.Iterate(
                        SuperSimpleExpressionParser.Parse(scopeStack, currentToken),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new ParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (currentToken == "EndEach")
                {
                    scopeStack.RemoveFirst();
                }
                else if (currentToken.StartsWith("If.") || currentToken.StartsWith("IfNotNull."))
                {
                    currentToken = currentToken.Substring(currentToken.IndexOf('.') + 1);
                    var condition = SyntaxTree.Conditional(
                        SuperSimpleExpressionParser.Parse(scopeStack, currentToken),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(condition);
                    scopeStack.AddFirst(new ParserScope { Block = condition.TrueBlock, ModelType = scopeStack.First.Value.ModelType });
                }
                else if (currentToken.StartsWith("IfNot.") || currentToken.StartsWith("IfNull."))
                {
                    currentToken = currentToken.Substring(currentToken.IndexOf('.') + 1);
                    var condition = SyntaxTree.Conditional(
                        SuperSimpleExpressionParser.Parse(scopeStack, currentToken),
                        SyntaxTree.Block(),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(condition);
                    scopeStack.AddFirst(new ParserScope { Block = condition.FalseBlock, ModelType = scopeStack.First.Value.ModelType });
                }
                else if (currentToken == "EndIf")
                {
                    scopeStack.RemoveFirst();
                }
                else if (currentToken.StartsWith("Partial"))
                {
                    var details = GetNameAndModelFromToken(currentToken);
                    ExpressionNode modelExpression = Expression.Self(scopeStack.First.Value.ModelType);

                    if (!String.IsNullOrEmpty(details.Item2))
                    {
                        modelExpression = SuperSimpleExpressionParser.Parse(scopeStack, details.Item2);
                    }
                    scopeStack.First.Value.Block.Add(SyntaxTree.Include(details.Item1, modelExpression));
                }
                else if (currentToken.StartsWith("Section"))
                {
                    scopeStack.First.Value.Block.Add(SyntaxTree.Override(GetNameAndModelFromToken(currentToken).Item1));
                }
                else if (currentToken == "Flush")
                {
                    scopeStack.First.Value.Block.Add(SyntaxTree.Flush());
                }
                else if (currentToken.StartsWith("!"))
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack, currentToken.Substring(1));
                    scopeStack.First.Value.Block.Add(SyntaxTree.WriteExpression(expression, true));
                }
                else
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack, currentToken);
                    scopeStack.First.Value.Block.Add(SyntaxTree.WriteExpression(expression));
                }
            }

            return scopeStack.First.Value.Block;
        }

        private ExtendTemplateNode ParseMaster(IEnumerable<SuperSimpleToken> tokens, Type modelType)
        {
            var masterName = "";
            var sections = new Dictionary<string, SyntaxTreeNode>();
            var currentSectionTokens = new List<SuperSimpleToken>();
            var sectionName = "";
            var inSection = false;

            foreach (var token in tokens)
            {
                var currentToken = token.Content.Trim(new[] { '@', ';' });
                if (currentToken.StartsWith("Master["))
                {
                    masterName = GetNameAndModelFromToken(currentToken).Item1;
                }
                else if (currentToken.StartsWith("Section[") && !inSection)
                {
                    sectionName = GetNameAndModelFromToken(currentToken).Item1;
                    inSection = true;
                }
                else if (currentToken == "EndSection")
                {
                    var block = ParseTemplate(currentSectionTokens, modelType);
                    sections.Add(sectionName, block);
                    inSection = false;
                    currentSectionTokens.Clear();
                }
                else if (inSection)
                {
                    currentSectionTokens.Add(token);
                }
            }
            return SyntaxTree.Extend(masterName, sections);
        }

        public Tuple<string, string> GetNameAndModelFromToken(string token)
        {
            var match = NameMatcher.Match(token);
            return Tuple.Create(match.Groups["Name"].Value, match.Groups["Model"].Value);
        }

        internal class ParserScope
        {
            public BlockNode Block { get; set; }

            public Type ModelType { get; set; }
        }
    }
}