using System;
using System.IO;
using Veil.Compiler;
using Veil.Parser;

namespace Veil
{
    public class VeilEngine : IVeilEngine
    {
        private readonly ITemplateParser parser;

        private readonly ITemplateCompiler compiler;

        public VeilEngine()
        {
            this.compiler = new VeilTemplateCompiler();
        }

        public Action<TextWriter, T> Compile<T>(TextReader templateContents)
        {
            var syntaxTree = this.parser.Parse(templateContents, typeof(T));
            return this.compiler.Compile<T>(syntaxTree);
        }
    }
}