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
            emitter.LoadModelToStack();

            var get = node.ModelProperty.GetGetMethod();
            if (get.IsVirtual)
            {
                emitter.CallVirtual(get);
            }
            else
            {
                emitter.Call(get);
            }

            emitter.CallWriteFor(node.ModelProperty.PropertyType);
        }
    }
}