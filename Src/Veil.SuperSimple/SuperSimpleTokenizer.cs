using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleTokenizer
    {
        private static readonly Regex sueprsimple = new Regex(@"@!?(Model|Current|If(Not)?(Null)?|EndIf|Each|EndEach|Partial|Master|Section|EndSection|Flush)(\.[a-zA-Z0-9-_\.]*)?(\[.*?\])?;?", RegexOptions.Compiled);

        public static IEnumerable<SuperSimpleToken> Tokenize(TextReader templateReader)
        {
            var template = templateReader.ReadToEnd();
            var matches = sueprsimple.Matches(template);
            var index = 0;
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    yield return new SuperSimpleToken(false, template.Substring(index, match.Index - index));
                }
                yield return new SuperSimpleToken(true, match.Value.Trim(' ', '\t', '@', ';'));
                index = match.Index + match.Length;
            }
            if (index < template.Length)
            {
                yield return new SuperSimpleToken(false, template.Substring(index));
            }
        }
    }

    internal struct SuperSimpleToken
    {
        private bool isSyntaxToken;
        private string content;

        public SuperSimpleToken(bool isSyntaxToken, string content)
        {
            this.isSyntaxToken = isSyntaxToken;
            this.content = content;
        }

        public bool IsSyntaxToken { get { return this.isSyntaxToken; } }

        public string Content { get { return this.content; } }
    }
}