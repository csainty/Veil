using System;
using System.Collections.Generic;

namespace Nancy.ViewEngines.Veil.Handlebars
{
    public class HandlebarsViewEngineRegistration : INancyVeilViewEngineRegistration
    {
        public IEnumerable<string> Extensions
        {
            get { return new[] { "haml" }; }
        }

        public Func<global::Veil.Parser.ITemplateParser> ParserFactory
        {
            get { return () => new global::Veil.Handlebars.HandlebarsParser(); }
        }
    }
}