using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Nancy.ViewEngines.Veil
{
    public interface INancyVeilViewEngineRegistration
    {
        IEnumerable<string> Extensions { get; }

        Func<ITemplateParser> ParserFactory { get; }
    }
}