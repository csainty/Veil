using System;

namespace Veil.Compiler
{
    public class VeilCompilerException : Exception
    {
        public VeilCompilerException(string message)
            : base(message)
        {
        }
    }
}