using System;
using System.Linq;
using System.Reflection;
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
            this.method = new DynamicMethod(methodName, invokeMethod.ReturnType, invokeMethod.GetParameters().Select(x => x.ParameterType).ToArray(), true);
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
                case 0: this.generator.Emit(OpCodes.Ldarg_0); return;
                case 1: this.generator.Emit(OpCodes.Ldarg_1); return;
                case 2: this.generator.Emit(OpCodes.Ldarg_2); return;
                case 3: this.generator.Emit(OpCodes.Ldarg_3); return;
                default: throw new InvalidOperationException("Tried to emit a load argument for an unspoorted index.");
            }
        }

        public void LoadConstant(string value)
        {
            this.generator.Emit(OpCodes.Ldstr, value);
        }

        public void LoadConstant(bool value)
        {
            LoadConstant(value ? 1 : 0);
        }

        public void LoadConstant(int value)
        {
            switch (value)
            {
                case -1: this.generator.Emit(OpCodes.Ldc_I4_M1); return;
                case 0: this.generator.Emit(OpCodes.Ldc_I4_0); return;
                case 1: this.generator.Emit(OpCodes.Ldc_I4_1); return;
                case 2: this.generator.Emit(OpCodes.Ldc_I4_2); return;
                case 3: this.generator.Emit(OpCodes.Ldc_I4_3); return;
                case 4: this.generator.Emit(OpCodes.Ldc_I4_4); return;
                case 5: this.generator.Emit(OpCodes.Ldc_I4_5); return;
                case 6: this.generator.Emit(OpCodes.Ldc_I4_6); return;
                case 7: this.generator.Emit(OpCodes.Ldc_I4_7); return;
                case 8: this.generator.Emit(OpCodes.Ldc_I4_8); return;
            }

            if (value >= SByte.MinValue && value <= SByte.MaxValue)
            {
                this.generator.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                return;
            }

            this.generator.Emit(OpCodes.Ldc_I4, value);
        }

        internal Local DeclareLocal(Type type)
        {
            return new Local(this.generator.DeclareLocal(type));
        }

        internal void StoreLocal(Local local)
        {
            switch (local.Index)
            {
                case 0: this.generator.Emit(OpCodes.Stloc_0); return;
                case 1: this.generator.Emit(OpCodes.Stloc_1); return;
                case 2: this.generator.Emit(OpCodes.Stloc_2); return;
                case 3: this.generator.Emit(OpCodes.Stloc_3); return;
            }

            if (local.Index >= SByte.MinValue && local.Index <= SByte.MaxValue)
            {
                this.generator.Emit(OpCodes.Stloc_S, (sbyte)local.Index);
                return;
            }

            this.generator.Emit(OpCodes.Stloc, local.Index);
        }

        internal void LoadLocalAddress(Local local)
        {
            if (local.Index >= SByte.MinValue && local.Index <= SByte.MaxValue)
            {
                this.generator.Emit(OpCodes.Ldloca_S, (sbyte)local.Index);
                return;
            }

            this.generator.Emit(OpCodes.Ldloca, local.Index);
        }

        internal void LoadLocal(Local local)
        {
            switch (local.Index)
            {
                case 0: this.generator.Emit(OpCodes.Ldloc_0); return;
                case 1: this.generator.Emit(OpCodes.Ldloc_1); return;
                case 2: this.generator.Emit(OpCodes.Ldloc_2); return;
                case 3: this.generator.Emit(OpCodes.Ldloc_3); return;
            }

            if (local.Index >= SByte.MinValue && local.Index <= SByte.MaxValue)
            {
                this.generator.Emit(OpCodes.Ldloc_S, (sbyte)local.Index);
                return;
            }

            this.generator.Emit(OpCodes.Ldloc, local.Index);
        }

        internal void CallVirtual(MethodInfo info, Type constrainedType = null)
        {
            if (constrainedType != null)
            {
                this.generator.Emit(OpCodes.Constrained, constrainedType);
            }
            this.generator.Emit(OpCodes.Callvirt, info);
        }

        internal void Call(MethodInfo info)
        {
            this.generator.Emit(OpCodes.Call, info);
        }

        internal void CastClass(Type type)
        {
            this.generator.Emit(OpCodes.Castclass, type);
        }

        internal void LoadField(FieldInfo fieldInfo)
        {
            this.generator.Emit(OpCodes.Ldfld, fieldInfo);
        }

        internal Label DefineLabel()
        {
            return this.generator.DefineLabel();
        }

        internal void MarkLabel(Label label)
        {
            this.generator.MarkLabel(label);
        }

        internal void Branch(Label label)
        {
            this.generator.Emit(OpCodes.Br, label);
        }

        internal void BranchIfFalse(Label label)
        {
            this.generator.Emit(OpCodes.Brfalse, label);
        }

        internal void BranchIfTrue(Label label)
        {
            this.generator.Emit(OpCodes.Brtrue, label);
        }

        internal void CompareEqual()
        {
            this.generator.Emit(OpCodes.Ceq);
        }

        internal void Pop()
        {
            this.generator.Emit(OpCodes.Pop);
        }

        internal void Add()
        {
            this.generator.Emit(OpCodes.Add);
        }

        internal void LoadLength(Type elementType)
        {
            this.generator.Emit(OpCodes.Ldlen);
        }

        internal void LoadElement(Type elementType)
        {
            if (!elementType.IsValueType) this.generator.Emit(OpCodes.Ldelem_Ref);
            else if (elementType == typeof(sbyte)) this.generator.Emit(OpCodes.Ldelem_I1);
            else if (elementType == typeof(byte)) this.generator.Emit(OpCodes.Ldelem_U1);
            else if (elementType == typeof(short)) this.generator.Emit(OpCodes.Ldelem_I2);
            else if (elementType == typeof(ushort)) this.generator.Emit(OpCodes.Ldelem_U2);
            else if (elementType == typeof(int)) this.generator.Emit(OpCodes.Ldelem_I4);
            else if (elementType == typeof(uint)) this.generator.Emit(OpCodes.Ldelem_U4);
            else if (elementType == typeof(long)) this.generator.Emit(OpCodes.Ldelem_I8);
            else if (elementType == typeof(float)) this.generator.Emit(OpCodes.Ldelem_R4);
            else if (elementType == typeof(double)) this.generator.Emit(OpCodes.Ldelem_R8);
            else this.generator.Emit(OpCodes.Ldelem);
        }

        internal void CompareLessThan()
        {
            this.generator.Emit(OpCodes.Clt);
        }

#if DEBUG

        internal void Debug(Local local)
        {
            generator.EmitWriteLine(local.Builder);
        }

        internal void Debug(string value)
        {
            generator.EmitWriteLine(value);
        }

#endif
    }
}