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

        public static void CallMethod<T>(this Emit<Action<TextWriter, T>> emitter, MethodInfo info, Type typeBeingCalledOn)
        {
            if (info.IsVirtual && typeBeingCalledOn.IsValueType)
            {
                using (var holder = emitter.DeclareLocal(typeBeingCalledOn))
                {
                    emitter.StoreLocal(holder);
                    emitter.LoadLocalAddress(holder);
                    emitter.CallVirtual(info, typeBeingCalledOn);
                }
            }
            else
            {
                CallMethod(emitter, info);
            }
        }
    }
}