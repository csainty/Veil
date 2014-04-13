using System;
using System.IO;
using Veil.Compiler;

namespace Veil
{
    public class VeilEngine : IVeilEngine
    {
        private readonly ITemplateParser parser;

        private readonly ITemplateCompiler compiler;

        public VeilEngine(ITemplateParser parser)
        {
            this.parser = parser;
            this.compiler = new VeilTemplateCompiler();
        }

        public Action<TextWriter, T> Compile<T>(TextReader templateContents)
        {
            var syntaxTree = this.parser.Parse(templateContents, typeof(T));
            return this.compiler.Compile<T>(syntaxTree);
        }
    }
}