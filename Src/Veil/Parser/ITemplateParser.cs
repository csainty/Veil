using System;
using System.IO;

namespace Veil.Parser
{
    public interface ITemplateParser
    {
        SyntaxTreeNode Parse(TextReader templateReader, Type modelType);
    }
}