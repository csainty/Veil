using System.IO;

namespace Veil
{
    /// <summary>
    /// Connects Veil to its hosting environment
    /// </summary>
    public interface IVeilContext
    {
        /// <summary>
        /// Loads the contents of a named template.
        /// Used for includes.
        /// </summary>
        /// <param name="name">The name of the template to load</param>
        /// <param name="templateType">The parser that is loading the template</param>
        TextReader GetTemplateByName(string name, string templateType);
    }
}