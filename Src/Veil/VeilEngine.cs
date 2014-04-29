using System;
using System.Collections.Generic;
using System.IO;
using Veil.Compiler;

namespace Veil
{
    public class VeilEngine : IVeilEngine
    {
        private static IDictionary<string, ITemplateParser> Parsers = new Dictionary<string, ITemplateParser>();

        public Action<TextWriter, T> Compile<T>(string templateType, TextReader templateContents)
        {
            if (String.IsNullOrEmpty(templateType)) throw new ArgumentNullException("templateType");
            if (templateContents == null) throw new ArgumentNullException("templateContents");
            if (!Parsers.ContainsKey(templateType)) throw new ArgumentException("A parser for templateType '{0}' is not registered.".FormatInvariant(templateType), "templateType");

            var syntaxTree = Parsers[templateType].Parse(templateContents, typeof(T));
            return new VeilTemplateCompiler<T>().Compile(syntaxTree);
        }

        public static void RegisterParser(string templateType, ITemplateParser parser)
        {
            if (String.IsNullOrEmpty(templateType)) throw new ArgumentNullException("templateType");
            if (Parsers.ContainsKey(templateType)) throw new ArgumentException("A parser for templateType '{0}' ({1}) is already registered.".FormatInvariant(templateType, Parsers[templateType].GetType().Name), "templateType");

            Parsers.Add(templateType, parser);
        }

        public static void ClearParserRegistrations()
        {
            Parsers.Clear();
        }
    }
}