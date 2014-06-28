using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil.Handlebars
{
    public class HandlebarsTemplateParserRegistration : ITemplateParserRegistration
    {
        public IEnumerable<string> Keys
        {
            get { return new[] { "haml", "handlebars" }; }
        }

        public Func<ITemplateParser> ParserFactory
        {
            get { return () => new HandlebarsParser(); }
        }
    }
}