using System;
using System.IO;
using Veil.Compiler;
using Veil.Parser;

namespace Veil
{
    public class VeilEngine
    {
        private readonly ITemplateParser parser;

        private readonly ITemplateCompiler compiler;

        public Action<TextWriter, T> Compile<T>(TextReader templateContents)
        {
            var syntaxTree = this.parser.Parse(templateContents, typeof(T));
            return this.compiler.Compile<T>(syntaxTree);
        }
    }
}