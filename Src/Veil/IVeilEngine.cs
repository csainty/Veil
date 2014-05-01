using System;
using System.IO;

namespace Veil
{
    /// <summary>
    /// An interface for <see cref="VeilEngine"/> provided for testers
    /// </summary>
    public interface IVeilEngine
    {
        Action<TextWriter, T> Compile<T>(string templateType, TextReader templateContents);
    }
}