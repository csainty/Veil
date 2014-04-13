using System;
using System.IO;

namespace Veil
{
    public interface ITemplateParser
    {
        TemplateRootNode Parse(TextReader templateReader, Type modelType);
    }
}