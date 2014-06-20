using System;
using System.Collections.Generic;
using System.IO;
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
        // TODO: A serious refactor / rewrite is needed for this class
        private static Regex SuperSimpleMatcher = new Regex(@"@!?(Model|Current|If(Not)?(Null)?|EndIf|Each|EndEach|Partial|Master|Section|EndSection|Flush)(\.[a-zA-Z0-9-_\.]*)?(\[.*?\])?;?", RegexOptions.Compiled);

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
            scopeStack.AddFirst(new ParserScope { Block = SyntaxTree.Block(), ModelType = modelType });

            var matches = SuperSimpleMatcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    scopeStack.First.Value.Block.Add(SyntaxTree.WriteString(template.Substring(index, match.Index - index)));
                }
                index = match.Index + match.Length;

                var token = match.Value.Trim(new[] { '@', ';' });
                if (token == "Each")
                {
                    var each = SyntaxTree.Iterate(
                        Expression.Self(scopeStack.First.Value.ModelType),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new ParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (token.StartsWith("Each"))
                {
                    token = token.Substring(5);
                    var each = SyntaxTree.Iterate(
                        SuperSimpleExpressionParser.Parse(scopeStack, token),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new ParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (token == "EndEach")
                {
                    scopeStack.RemoveFirst();
                }
                else if (token.StartsWith("If.") || token.StartsWith("IfNotNull."))
                {
                    token = token.Substring(token.IndexOf('.') + 1);
                    var condition = SyntaxTree.Conditional(
                        SuperSimpleExpressionParser.Parse(scopeStack, token),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(condition);
                    scopeStack.AddFirst(new ParserScope { Block = condition.TrueBlock, ModelType = scopeStack.First.Value.ModelType });
                }
                else if (token.StartsWith("IfNot.") || token.StartsWith("IfNull."))
                {
                    token = token.Substring(token.IndexOf('.') + 1);
                    var condition = SyntaxTree.Conditional(
                        SuperSimpleExpressionParser.Parse(scopeStack, token),
                        SyntaxTree.Block(),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(condition);
                    scopeStack.AddFirst(new ParserScope { Block = condition.FalseBlock, ModelType = scopeStack.First.Value.ModelType });
                }
                else if (token == "EndIf")
                {
                    scopeStack.RemoveFirst();
                }
                else if (token.StartsWith("Partial"))
                {
                    var details = GetNameAndModelFromToken(token);
                    ExpressionNode modelExpression = Expression.Self(scopeStack.First.Value.ModelType);

                    if (!String.IsNullOrEmpty(details.Item2))
                    {
                        modelExpression = SuperSimpleExpressionParser.Parse(scopeStack, details.Item2);
                    }
                    scopeStack.First.Value.Block.Add(SyntaxTree.Include(details.Item1, modelExpression));
                }
                else if (token.StartsWith("Section"))
                {
                    scopeStack.First.Value.Block.Add(SyntaxTree.Override(GetNameAndModelFromToken(token).Item1));
                }
                else if (token == "Flush")
                {
                    scopeStack.First.Value.Block.Add(SyntaxTree.Flush());
                }
                else if (token.StartsWith("!"))
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack, token.Substring(1));
                    scopeStack.First.Value.Block.Add(SyntaxTree.WriteExpression(expression, true));
                }
                else
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack, token);
                    scopeStack.First.Value.Block.Add(SyntaxTree.WriteExpression(expression));
                }
            }
            if (index < template.Length)
            {
                scopeStack.First.Value.Block.Add(SyntaxTree.WriteString(template.Substring(index)));
            }

            return scopeStack.First.Value.Block;
        }

        private ExtendTemplateNode ParseMaster(string template, Type modelType)
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