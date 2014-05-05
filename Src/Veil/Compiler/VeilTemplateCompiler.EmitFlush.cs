using System.IO;
using System.Reflection;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static MethodInfo flushMethod = typeof(TextWriter).GetMethod("Flush");

        private void EmitFlush()
        {
            LoadWriterToStack();
            emitter.CallMethod(flushMethod);
        }
    }
}