using System;
using System.IO;

namespace Veil
{
    internal class NullVeilContext : IVeilContext
    {
        public TextReader GetTemplateByName(string name, string parserKey)
        {
            throw new InvalidOperationException("You have attempted to compile a template that uses \"includes\" without specifying an IVeilContext in your VeilEngine constructor. You need an IVeilEngine to allow the compiler to load named templates.");
        }
    }
}