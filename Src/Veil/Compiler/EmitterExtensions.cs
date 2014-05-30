using System;
using System.IO;
using System.Reflection;

namespace Veil.Compiler
{
    internal static class EmitterExtensions
    {
        public static void CallMethod<T>(this Emit<Action<TextWriter, T>> emitter, MethodInfo info)
        {
            if (info.IsVirtual)
            {
                emitter.CallVirtual(info);
            }
            else
            {
                emitter.Call(info);
            }
        }
    }
}