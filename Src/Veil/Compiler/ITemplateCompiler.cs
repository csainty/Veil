using System;
using System.IO;

namespace Veil.Compiler
{
    internal interface ITemplateCompiler
    {
        Action<TextWriter, T> Compile<T>(SyntaxTreeNode templateSyntaxTree);
    }
}