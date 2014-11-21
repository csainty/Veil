using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil.Handlebars
{
    /// <summary>
    /// Used to auto-register this parser. You should not need touch it.
    /// </summary>
    public class HandlebarsTemplateParserRegistration : ITemplateParserRegistration
    {
        public IEnumerable<string> Keys
        {
            get { return new[] { "handlebars", "hbs" }; }
        }

        public Func<ITemplateParser> ParserFactory
        {
            get { return () => new HandlebarsParser(); }
        }
    }
}