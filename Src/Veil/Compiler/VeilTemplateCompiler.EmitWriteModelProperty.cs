using System;
using System.IO;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitWriteModelProperty<T>(Emit<Action<TextWriter, T>> emitter, WriteModelPropertyNode node)
        {
            emitter.LoadWriterToStack();
            emitter.LoadModelPropertyToStack(node.Property);
            emitter.CallWriteFor(node.Property.Type);
        }
    }
}