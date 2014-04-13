using System;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler : ITemplateCompiler
    {
        public Action<TextWriter, T> Compile<T>(TemplateRootNode templateSyntaxTree)
        {
            var state = new VeilCompilerState<T>();

            EmitNode(state, templateSyntaxTree);

            state.Emitter.Return();
            return state.Emitter.CreateDelegate();
        }
    }
}