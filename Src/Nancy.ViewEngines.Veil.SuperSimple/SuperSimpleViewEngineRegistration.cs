using System;
using System.Collections.Generic;

namespace Nancy.ViewEngines.Veil.SuperSimple
{
    public class SuperSimpleViewEngineRegistration : INancyVeilViewEngineRegistration
    {
        public IEnumerable<string> Extensions
        {
            get { return new[] { "vsshtml" }; }
        }

        public Func<global::Veil.Parser.ITemplateParser> ParserFactory
        {
            get { return () => new global::Veil.SuperSimple.SuperSimpleParser(); }
        }
    }
}