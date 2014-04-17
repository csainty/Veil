using System;
using System.IO;

namespace Veil.SuperSimple
{
    public class SuperSimpleParser : ITemplateParser
    {
        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var template = templateReader.ReadToEnd();

            return SyntaxTreeNode.Block();
        }
    }
}