using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                    blockStack.Peek().Add(WriteStringLiteral(template.Substring(index, match.Index - index)));
                }

                index = match.Index + match.Length;

                var token = match.Value.Trim(new[] { '{', '}', ' ', '\t' });

                if (token.StartsWith("#if"))
                {
                    var block = new BlockNode();
                    var conditional = new ConditionalOnModelPropertyNode
                    {
                        ModelProperty = ParsePropertyName(token.Substring(4), modelType),
                        TrueBlock = block
                    };
                    blockStack.Peek().Add(conditional);
                    blockStack.Push(block);
                }
                else if (token == "else")
                {
                    AssertInsideConditionalOnModelBlock(blockStack, "{{else}}");
                    blockStack.Pop();
                    var block = new BlockNode();
                    ((ConditionalOnModelPropertyNode)blockStack.Peek().Nodes.Last()).FalseBlock = block;
                    blockStack.Push(block);
                }
                else if (token == "/if")
                {
                    AssertInsideConditionalOnModelBlock(blockStack, "{{/if}}");
                    blockStack.Pop();
                }
                else
                {
                    blockStack.Peek().Add(WriteModelProperty(ParsePropertyName(token, modelType)));
                }
            }
            if (index < template.Length)
            {
                blockStack.Peek().Add(WriteStringLiteral(template.Substring(index)));
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
                faulted = !(blockStack.Peek().Nodes.Last() is ConditionalOnModelPropertyNode);
                blockStack.Push(block);
            }

            if (faulted)
            {
                throw new VeilParserException("Found token '{0}' outside of a conditional block.".FormatInvariant(foundToken));
            }
        }

        private static PropertyInfo ParsePropertyName(string name, Type modelType)
        {
            name = name.Trim();
            var propertyInfo = modelType.GetProperty(name);
            if (propertyInfo == null) throw new VeilParserException("Property '{0}' not found on model '{1}'".FormatInvariant(name, modelType.Name));
            return propertyInfo;
        }

        private static ISyntaxTreeNode WriteStringLiteral(string content)
        {
            return new WriteLiteralNode
            {
                LiteralType = typeof(string),
                LiteralContent = content
            };
        }

        private static ISyntaxTreeNode WriteModelProperty(PropertyInfo property)
        {
            return new WriteModelPropertyNode
            {
                ModelProperty = property
            };
        }
    }
}