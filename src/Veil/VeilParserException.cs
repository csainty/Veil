using System;

namespace Veil
{
    /// <summary>
    /// Represents general errors during parsing templates.
    /// </summary>
    public class VeilParserException : Exception
    {
        /// <summary>
        /// Create an exception with the supplied message
        /// </summary>
        public VeilParserException(string message)
            : base(message)
        {
        }
    }
}