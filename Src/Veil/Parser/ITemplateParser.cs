using System;
using System.IO;

namespace Veil
{
    public interface ITemplateParser
    {
        SyntaxTreeNode Parse(TextReader templateReader, Type modelType);
    }
}