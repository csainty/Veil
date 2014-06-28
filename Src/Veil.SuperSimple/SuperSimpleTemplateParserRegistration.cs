using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil.SuperSimple
{
    public class SuperSimpleTemplateParserRegistration : ITemplateParserRegistration
    {
        public IEnumerable<string> Keys
        {
            get { return new[] { "vsshtml", "sshtml", "supersimple" }; }
        }

        public Func<ITemplateParser> ParserFactory
        {
            get { return () => new SuperSimpleParser(); }
        }
    }
}