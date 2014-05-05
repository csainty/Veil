using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Veil.Parser;

namespace Veil.SuperSimple
{
    // TODO:
    // - Stack assertions for template end
    // - Stack assertions for @If
    // - Stack assertions for @Each
    public class SuperSimpleParser : ITemplateParser
    {
        private static Regex SuperSimpleMatcher = new Regex(@"@!?[A-Za-z0-9\.]*(\[.*?\])?;?", RegexOptions.Compiled);
        private static Regex NameMatcher = new Regex(@".*?\[\'(?<Name>.*?)\'(,(?<Model>.*))?\]", RegexOptions.Compiled);

        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var template = templateReader.ReadToEnd().Trim();
            if (template.StartsWith("@Master"))
            {
                return ParseMaster(template, modelType);
            }

            return ParseTemplate(template, modelType);
        }

        private SyntaxTreeNode ParseTemplate(string template, Type modelType)
        {
            var scopeStack = new LinkedList<ParserScope>();
            scopeStack.AddFirst(new ParserScope { Block = SyntaxTreeNode.Block(), ModelType = modelType });

            var matches = SuperSimpleMatcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    scopeStack.First.Value.Block.Add(SyntaxTreeNode.WriteString(template.Substring(index, match.Index - index)));
                }
                index = match.Index + match.Length;

                var token = match.Value.Trim(new[] { '@', ';' });
                if (token == "Each")
                {
                    var each = SyntaxTreeNode.Iterate(
                        SyntaxTreeNode.ExpressionNode.Self(scopeStack.First.Value.ModelType),
                        SyntaxTreeNode.Block()
                    );
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new ParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (token.StartsWith("Each."))
                {
                    token = token.Substring(5);
                    var each = SyntaxTreeNode.Iterate(
                        SuperSimpleExpressionParser.Parse(scopeStack, token),
                        SyntaxTreeNode.Block()
                    );
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new ParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (token == "EndEach")
                {
                    scopeStack.RemoveFirst();
                }
                else if (token.StartsWith("If."))
                {
                    token = token.Substring(3);
                    var condition = SyntaxTreeNode.Conditional(
                        SuperSimpleExpressionParser.Parse(scopeStack, token),
                        SyntaxTreeNode.Block()
                    );
                    scopeStack.First.Value.Block.Add(condition);
                    scopeStack.AddFirst(new ParserScope { Block = condition.TrueBlock, ModelType = scopeStack.First.Value.ModelType });
                }
                else if (token.StartsWith("IfNot."))
                {
                    token = token.Substring(6);
                    var condition = SyntaxTreeNode.Conditional(
                        SuperSimpleExpressionParser.Parse(scopeStack, token),
                        SyntaxTreeNode.Block(),
                        SyntaxTreeNode.Block()
                    );
                    scopeStack.First.Value.Block.Add(condition);
                    scopeStack.AddFirst(new ParserScope { Block = condition.FalseBlock, ModelType = scopeStack.First.Value.ModelType });
                }
                else if (token == "EndIf")
                {
                    scopeStack.RemoveFirst();
                }
                else if (token.StartsWith("Partial["))
                {
                    var details = GetNameAndModelFromToken(token);
                    SyntaxTreeNode.ExpressionNode modelExpression = SyntaxTreeNode.ExpressionNode.Self(scopeStack.First.Value.ModelType);

                    if (!String.IsNullOrEmpty(details.Item2))
                    {
                        modelExpression = SuperSimpleExpressionParser.Parse(scopeStack, details.Item2);
                    }
                    scopeStack.First.Value.Block.Add(SyntaxTreeNode.Include(details.Item1, modelExpression));
                }
                else if (token.StartsWith("Section["))
                {
                    scopeStack.First.Value.Block.Add(SyntaxTreeNode.Override(GetNameAndModelFromToken(token).Item1));
                }
                else if (token.StartsWith("!"))
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack, token.Substring(1));
                    scopeStack.First.Value.Block.Add(SyntaxTreeNode.WriteExpression(expression, true));
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

        private SyntaxTreeNode.ExtendTemplateNode ParseMaster(string template, Type modelType)
        {
            var matches = SuperSimpleMatcher.Matches(template);
            var masterName = "";
            var sections = new Dictionary<string, SyntaxTreeNode>();
            var sectionStartindex = 0;
            var sectionName = "";
            var inSection = false;

            foreach (Match match in matches)
            {
                var token = match.Value.Trim(new[] { '@', ';' });
                if (token.StartsWith("Master["))
                {
                    masterName = GetNameAndModelFromToken(token).Item1;
                }
                else if (token.StartsWith("Section["))
                {
                    if (inSection) continue;
                    sectionName = GetNameAndModelFromToken(token).Item1;
                    sectionStartindex = match.Index + match.Length;
                    inSection = true;
                }
                else if (token == "EndSection")
                {
                    var block = Parse(new StringReader(template.Substring(sectionStartindex, match.Index - sectionStartindex)), modelType);
                    sections.Add(sectionName, block);
                    inSection = false;
                }
            }
            return SyntaxTreeNode.Extend(masterName, sections);
        }

        public Tuple<string, string> GetNameAndModelFromToken(string token)
        {
            var match = NameMatcher.Match(token);
            return Tuple.Create(match.Groups["Name"].Value, match.Groups["Model"].Value);
        }

        internal class ParserScope
        {
            public SyntaxTreeNode.BlockNode Block { get; set; }

            public Type ModelType { get; set; }
        }
    }
}