using System;
using System.IO;

namespace Veil.Parser
{
    internal interface ITemplateParser
    {
        TemplateRootNode Parse(TextReader templateReader, Type modelType);
    }
}