using System;

namespace Veil
{
    /// <summary>
    /// Represents general errors during parsing templates.
    /// </summary>
    public class VeilParserException : Exception
    {
        public VeilParserException(string message)
            : base(message)
        {
        }
    }
}