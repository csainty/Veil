using System;
using System.IO;

namespace Veil
{
    public interface IVeilEngine
    {
        Action<TextWriter, T> Compile<T>(TextReader templateContents);
    }
}