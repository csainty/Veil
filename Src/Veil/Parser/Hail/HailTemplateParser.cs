using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Veil.Parser.Hail
{
    internal class HailTemplateParser : ITemplateParser
    {
        public TemplateRootNode Parse(TextReader templateReader, Type modelType)
        {
            var template = templateReader.ReadToEnd();
            var blockStack = new Stack<BlockNode>();
            blockStack.Push(new TemplateRootNode());

            var matcher = new Regex(@"(?<!{){{[^{}]+}}(?!})");
            var matches = matcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    blockStack.Peek().Add(WriteLiteralNode.String(template.Substring(index, match.Index - index)));
                }

                index = match.Index + match.Length;

                var token = match.Value.Trim(new[] { '{', '}', ' ', '\t' });

                if (token.StartsWith("#if"))
                {
                    var block = new BlockNode();
                    var conditional = ConditionalOnModelExpressionNode.Create(modelType, token.Substring(4), block);
                    blockStack.Peek().Add(conditional);
                    blockStack.Push(block);
                }
                else if (token == "else")
                {
                    AssertInsideConditionalOnModelBlock(blockStack, "{{else}}");
                    blockStack.Pop();
                    var block = new BlockNode();
                    ((ConditionalOnModelExpressionNode)blockStack.Peek().Nodes.Last()).FalseBlock = block;
                    blockStack.Push(block);
                }
                else if (token == "/if")
                {
                    AssertInsideConditionalOnModelBlock(blockStack, "{{/if}}");
                    blockStack.Pop();
                }
                else
                {
                    blockStack.Peek().Add(WriteModelExpressionNode.Create(modelType, token));
                }
            }
            if (index < template.Length)
            {
                blockStack.Peek().Add(WriteLiteralNode.String(template.Substring(index)));
            }

            AssertStackOnRootNode(blockStack);

            return (TemplateRootNode)blockStack.Pop();
        }

        private void AssertStackOnRootNode(Stack<BlockNode> blockStack)
        {
            if (!(blockStack.Peek() is TemplateRootNode))
            {
                throw new VeilParserException("Mismatched block found. Expected to find the end of the template by found '{0}'".FormatInvariant(blockStack.Peek().GetType()));
            }
        }

        private void AssertInsideConditionalOnModelBlock(Stack<BlockNode> blockStack, string foundToken)
        {
            var faulted = false;
            faulted = blockStack.Count < 2;

            if (!faulted)
            {
                var block = blockStack.Pop();
                faulted = !(blockStack.Peek().Nodes.Last() is ConditionalOnModelExpressionNode);
                blockStack.Push(block);
            }

            if (faulted)
            {
                throw new VeilParserException("Found token '{0}' outside of a conditional block.".FormatInvariant(foundToken));
            }
        }
    }
}