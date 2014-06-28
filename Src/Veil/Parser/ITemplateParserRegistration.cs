using System;
using System.Collections.Generic;

namespace Veil.Parser
{
    public interface ITemplateParserRegistration
    {
        IEnumerable<string> Keys { get; }

        Func<ITemplateParser> ParserFactory { get; }
    }
}