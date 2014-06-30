using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil.SuperSimple
{
    /// <summary>
    /// Used to auto-register this parser. You should not need touch it.
    /// </summary>
    public class SuperSimpleParserRegistration : ITemplateParserRegistration
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