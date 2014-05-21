using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;

namespace Veil.Compiler
{
    internal class Emit<TDelegate> where TDelegate : class
    {
        private readonly DynamicMethod method;
        private readonly ILGenerator generator;
        private static int methodCount = 0;

        private Emit()
        {
            var delegateType = typeof(TDelegate);
            var invokeMethod = delegateType.GetMethod("Invoke");

            var methodName = "_DynamicMethod" + Interlocked.Increment(ref methodCount);
            this.method = new DynamicMethod(methodName, invokeMethod.ReturnType, invokeMethod.GetParameters().Select(x => x.ParameterType).ToArray());
            this.generator = this.method.GetILGenerator();
        }

        public static Emit<TDelegate> NewDynamicMethod()
        {
            return new Emit<TDelegate>();
        }

        public void Return()
        {
            this.generator.Emit(OpCodes.Ret);
        }

        public TDelegate CreateDelegate()
        {
            return (TDelegate)(object)this.method.CreateDelegate(typeof(TDelegate));
        }

        public void LoadArgument(int index)
        {
            switch (index)
            {
                case 0: this.generator.Emit(OpCodes.Ldarg_0); break;
                case 1: this.generator.Emit(OpCodes.Ldarg_1); break;
                case 2: this.generator.Emit(OpCodes.Ldarg_2); break;
                case 3: this.generator.Emit(OpCodes.Ldarg_3); break;
                default: throw new InvalidOperationException("Tried to emit a load argument for an unspoorted index.");
            }
        }

        public void LoadConstant(string value)
        {
            this.generator.Emit(OpCodes.Ldstr, value);
        }

        public void LoadConstant(double value)
        {
            this.generator.Emit(OpCodes.Ldc_R8, value);
        }

        public void LoadConstant(float value)
        {
            this.generator.Emit(OpCodes.Ldc_R4, value);
        }

        public void LoadConstant(long value)
        {
            this.generator.Emit(OpCodes.Ldc_I8, value);
        }

        public void LoadConstant(ulong value)
        {
            this.generator.Emit(OpCodes.Ldc_I8, value);
        }

        public void LoadConstant(int value)
        {
            switch (value)
            {
                case -1: this.generator.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: this.generator.Emit(OpCodes.Ldc_I4_0); break;
                case 1: this.generator.Emit(OpCodes.Ldc_I4_1); break;
                case 2: this.generator.Emit(OpCodes.Ldc_I4_2); break;
                case 3: this.generator.Emit(OpCodes.Ldc_I4_3); break;
                case 4: this.generator.Emit(OpCodes.Ldc_I4_4); break;
                case 5: this.generator.Emit(OpCodes.Ldc_I4_5); break;
                case 6: this.generator.Emit(OpCodes.Ldc_I4_6); break;
                case 7: this.generator.Emit(OpCodes.Ldc_I4_7); break;
                case 8: this.generator.Emit(OpCodes.Ldc_I4_8); break;
            }

            if (value >= SByte.MinValue && value <= SByte.MaxValue)
            {
                this.generator.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
            }
        }

        public void LoadConstant(uint value)
        {
            switch (value)
            {
                case -1: this.generator.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: this.generator.Emit(OpCodes.Ldc_I4_0); break;
                case 1: this.generator.Emit(OpCodes.Ldc_I4_1); break;
                case 2: this.generator.Emit(OpCodes.Ldc_I4_2); break;
                case 3: this.generator.Emit(OpCodes.Ldc_I4_3); break;
                case 4: this.generator.Emit(OpCodes.Ldc_I4_4); break;
                case 5: this.generator.Emit(OpCodes.Ldc_I4_5); break;
                case 6: this.generator.Emit(OpCodes.Ldc_I4_6); break;
                case 7: this.generator.Emit(OpCodes.Ldc_I4_7); break;
                case 8: this.generator.Emit(OpCodes.Ldc_I4_8); break;
            }
        }

    }
}