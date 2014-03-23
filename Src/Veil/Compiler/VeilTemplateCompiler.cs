using System;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler : ITemplateCompiler
    {
        public Action<TextWriter, T> Compile<T>(TemplateRootNode templateSyntaxTree)
        {
            var emitter = Emit<Action<TextWriter, T>>.NewDynamicMethod();

            EmitNode(emitter, templateSyntaxTree);

            emitter.Return();
            return emitter.CreateDelegate();
        }
    }
}