using System;

namespace Veil
{
    /// <summary>
    /// Represent general errors during compilation of templates
    /// </summary>
    public class VeilCompilerException : Exception
    {
        /// <summary>
        /// Creates an exception with the supplied messages
        /// </summary>
        public VeilCompilerException(string message)
            : base(message)
        {
        }
    }
}