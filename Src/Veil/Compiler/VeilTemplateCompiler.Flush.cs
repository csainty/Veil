using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo flushMethod = typeof(TextWriter).GetMethod("Flush");

        private Expression HandleFlush()
        {
            return Expression.Call(this.writer, flushMethod);
        }
    }
}