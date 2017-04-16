using System;
using System.IO;

namespace Veil
{
    /// <summary>
    /// An interface for <see cref="VeilEngine"/> provided for testers
    /// </summary>
    public interface IVeilEngine
    {
        /// <summary>
        /// Parses and compiles the specified template
        /// </summary>
        /// <typeparam name="T">The type of the model that will be passed to the template</typeparam>
        /// <param name="parserKey">Key of the <see cref="Veil.Parser.ITemplateParser"/> to use to parse the template.</param>
        /// <param name="templateContents">The contents of the template to compile</param>
        /// <returns>A compiled action ready to be executed as needed to render the template</returns>
        Action<TextWriter, T> Compile<T>(string parserKey, TextReader templateContents);

        /// <summary>
        /// Parses and compiles the specified template when the model type is not known
        /// </summary>
        /// <param name="parserKey">Key of the <see cref="Veil.Parser.ITemplateParser"/> to use to parse the template.</param>
        /// <param name="templateContents">The contents of the template to compile</param>
        /// <param name="modelType">The type of the model that will be passed to the template</param>
        /// <returns>A compiled action that will cast the model before execution</returns>
        Action<TextWriter, object> CompileNonGeneric(string parserKey, TextReader templateContents, Type modelType);
    }
}