using System;
using System.Reflection.Emit;

namespace Veil.Compiler
{
    internal class Local : IDisposable
    {
        private readonly LocalBuilder builder;

        public Local(LocalBuilder builder)
        {
            this.builder = builder;
        }

        public int Index { get { return this.builder.LocalIndex; } }

        public void Dispose()
        {
        }

        public LocalBuilder Builder { get { return this.builder; } }
    }
}