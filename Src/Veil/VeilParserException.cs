using System;

namespace Veil
{
    public class VeilParserException : Exception
    {
        public VeilParserException(string message)
            : base(message)
        {
        }
    }
}