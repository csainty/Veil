using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Veil.Handlebars
{
    internal static class HandlebarsTokenizer
    {
        private static readonly Regex handlebars = new Regex(@"(?<!{)({{[^{}]+}})|({{{[^{}]+}}})(?!})", RegexOptions.Compiled);

        public static IEnumerable<HandlebarsToken> Tokenize(TextReader templateReader)
        {
            var template = templateReader.ReadToEnd();
            var matches = handlebars.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    yield return new HandlebarsToken(false, template.Substring(index, match.Index - index));
                }
                yield return new HandlebarsToken(true, match.Value);
                index = match.Index + match.Length;
            }
            if (index < template.Length)
            {
                yield return new HandlebarsToken(false, template.Substring(index));
            }
        }
    }

    internal struct HandlebarsToken
    {
        private bool isSyntaxToken;
        private string content;

        public HandlebarsToken(bool isSyntaxToken, string content)
        {
            this.isSyntaxToken = isSyntaxToken;
            this.content = content;
        }

        public bool IsSyntaxToken { get { return this.isSyntaxToken; } }

        public string Content { get { return this.content; } }
    }
}