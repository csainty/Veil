using System;

namespace Veil
{
    /// <summary>
    /// Represent general errors during compilation of templates
    /// </summary>
    public class VeilCompilerException : Exception
    {
        public VeilCompilerException(string message)
            : base(message)
        {
        }
    }
}