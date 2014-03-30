using System;
using System.Collections.Generic;
using System.IO;
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
                else if (token.StartsWith("/if"))
                {
                    var nodes = blockStack.Pop();
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

            return (TemplateRootNode)blockStack.Pop();
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