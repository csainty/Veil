using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Veil.SuperSimple
{
    public class SuperSimpleParser : ITemplateParser
    {
        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var template = templateReader.ReadToEnd();
            var block = SyntaxTreeNode.Block();

            var matcher = new Regex(@"(@[^\s\<]*)");
            var matches = matcher.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    block.Add(SyntaxTreeNode.StringLiteral(template.Substring(index, match.Index - index)));
                }

                index = match.Index + match.Length;

                var token = match.Value.Trim(new[] { '@' });
                block.Add(SyntaxTreeNode.Expression(SuperSimpleExpressionParser.Parse(modelType, token)));
            }
            if (index < template.Length)
            {
                block.Add(SyntaxTreeNode.StringLiteral(template.Substring(index)));
            }

            return block;
        }
    }
}