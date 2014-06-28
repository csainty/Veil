using System;
using System.Collections.Generic;

namespace Veil.Parser
{
    /// <summary>
    /// Registers a parser automatically during startup
    /// </summary>
    public interface ITemplateParserRegistration
    {
        /// <summary>
        /// A list of keys this parser should be registered with
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// A factory method to create instances of the parser
        /// </summary>
        Func<ITemplateParser> ParserFactory { get; }
    }
}