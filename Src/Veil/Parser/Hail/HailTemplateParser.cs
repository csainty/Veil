using System;
using System.Collections.Generic;
using System.IO;

namespace Veil.Parser.Hail
{
    internal class HailTemplateParser : ITemplateParser
    {
        public TemplateRootNode Parse(TextReader templateReader, Type modelType)
        {
            var nodes = new List<ISyntaxTreeNode>();

            var template = templateReader.ReadToEnd();
            var state = ParserState.ReadingContent;
            var currentStartIndex = 0;
            var currentLength = 0;
            for (var i = 0; i < template.Length; i++)
            {
                char c = template[i];
                switch (state)
                {
                    case ParserState.ReadingContent:
                        if (c == '{') { state = ParserState.PossibleIdentifier; break; }
                        currentLength++;
                        break;

                    case ParserState.PossibleIdentifier:
                        if (c != '{') { state = ParserState.ReadingContent; currentLength += 2; break; }
                        state = ParserState.ReadingIdentifier;
                        if (currentLength > 0)
                        {
                            nodes.Add(WriteStringLiteral(template.Substring(currentStartIndex, currentLength)));
                            currentLength = 0;
                        }
                        currentStartIndex = i + 1;
                        break;

                    case ParserState.ReadingIdentifier:
                        if (c != '}') { currentLength++; break; }
                        state = ParserState.EndingIdentifier;
                        break;

                    case ParserState.EndingIdentifier:
                        if (c != '}') throw new VeilParserException("Expected }} Found '{0}'".FormatInvariant(c));
                        state = ParserState.ReadingContent;
                        var identifier = template.Substring(currentStartIndex, currentLength);
                        nodes.Add(ParseIdentifier(identifier, modelType));
                        currentStartIndex = i + 1;
                        currentLength = 0;
                        break;

                    default:
                        throw new VeilParserException("Unknown Parser State {0}".FormatInvariant(state));
                }
            }
            if (state == ParserState.ReadingContent && currentLength > 0)
            {
                nodes.Add(WriteStringLiteral(template.Substring(currentStartIndex, currentLength)));
            }

            return new TemplateRootNode
            {
                TemplateNodes = nodes
            };
        }

        private static ISyntaxTreeNode ParseIdentifier(string identifier, Type modelType)
        {
            return WriteModelProperty(identifier.Trim(), modelType);
        }

        private static ISyntaxTreeNode WriteStringLiteral(string content)
        {
            return new WriteLiteralNode
            {
                LiteralType = typeof(string),
                LiteralContent = content
            };
        }

        private static ISyntaxTreeNode WriteModelProperty(string propertyName, Type modelType)
        {
            var propertyInfo = modelType.GetProperty(propertyName);
            if (propertyInfo == null) throw new VeilParserException("Property '{0}' not found on model '{1}'".FormatInvariant(propertyName, modelType.Name));
            return new WriteModelPropertyNode
            {
                ModelProperty = propertyInfo
            };
        }
    }
}