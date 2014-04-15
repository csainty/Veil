using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Veil.Handlebars
{
    public class HandlebarsParser : ITemplateParser
    {
        static HandlebarsParser()
        {
            VeilEngine.RegisterParser("handlebars", new HandlebarsParser());
        }

        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var template = templateReader.ReadToEnd();
            var blockStack = new Stack<Veil.SyntaxTreeNode.BlockNode>();
            blockStack.Push(SyntaxTreeNode.Block());

            var matcher = new Regex(@"(?<!{){{[^{}]+}}(?!})");
            var matches = matcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    blockStack.Peek().Add(SyntaxTreeNode.StringLiteral(template.Substring(index, match.Index - index)));
                }

                index = match.Index + match.Length;

                var token = match.Value.Trim(new[] { '{', '}', ' ', '\t' });

                if (token.StartsWith("#if"))
                {
                    var block = SyntaxTreeNode.Block();
                    var conditional = SyntaxTreeNode.Conditional(ExpressionParser.Parse(modelType, token.Substring(4)), block);
                    blockStack.Peek().Add(conditional);
                    blockStack.Push(block);
                }
                else if (token == "else")
                {
                    AssertInsideConditionalOnModelBlock(blockStack, "{{else}}");
                    blockStack.Pop();
                    var block = SyntaxTreeNode.Block();
                    ((SyntaxTreeNode.ConditionalOnModelExpressionNode)blockStack.Peek().Nodes.Last()).FalseBlock = block;
                    blockStack.Push(block);
                }
                else if (token == "/if")
                {
                    AssertInsideConditionalOnModelBlock(blockStack, "{{/if}}");
                    blockStack.Pop();
                }
                else
                {
                    blockStack.Peek().Add(SyntaxTreeNode.Expression(ExpressionParser.Parse(modelType, token)));
                }
            }
            if (index < template.Length)
            {
                blockStack.Peek().Add(SyntaxTreeNode.StringLiteral(template.Substring(index)));
            }

            AssertStackOnRootNode(blockStack);

            return blockStack.Pop();
        }

        private void AssertStackOnRootNode(Stack<Veil.SyntaxTreeNode.BlockNode> blockStack)
        {
            if (blockStack.Count != 1)
            {
                throw new VeilParserException(String.Format("Mismatched block found. Expected to find the end of the template but found '{0}' open blocks.", blockStack.Count));
            }
        }

        private void AssertInsideConditionalOnModelBlock(Stack<SyntaxTreeNode.BlockNode> blockStack, string foundToken)
        {
            var faulted = false;
            faulted = blockStack.Count < 2;

            if (!faulted)
            {
                var block = blockStack.Pop();
                faulted = !(blockStack.Peek().Nodes.Last() is SyntaxTreeNode.ConditionalOnModelExpressionNode);
                blockStack.Push(block);
            }

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a conditional block.", foundToken));
            }
        }
    }
}