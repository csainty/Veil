using System;
using System.IO;
using System.Linq;
using Veil.Parser;

namespace Veil.SuperSimple
{
    /// <summary>
    /// A Veil parser for the SuperSimple syntax
    /// </summary>
    public class SuperSimpleParser : ITemplateParser
    {
        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var tokens = SuperSimpleTokenizer.Tokenize(templateReader).ToArray();
            var firstToken = tokens.First();
            if (firstToken.IsSyntaxToken && firstToken.Content.StartsWith("Master"))
            {
                return SuperSimpleMasterPageParser.Parse(tokens, modelType);
            }

            return SuperSimpleTemplateParser.Parse(tokens, modelType);
        }
    }
}