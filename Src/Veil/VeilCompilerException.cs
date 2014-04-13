using System;

namespace Veil
{
    public class VeilCompilerException : Exception
    {
        public VeilCompilerException(string message)
            : base(message)
        {
        }
    }
}