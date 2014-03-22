using System;

namespace Veil.Parser
{
    public class VeilParserException : Exception
    {
        public VeilParserException(string message)
            : base(message)
        {
        }
    }
}