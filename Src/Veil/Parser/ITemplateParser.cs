using System;
using System.IO;

namespace Veil.Parser
{
    /// <summary>
    /// Defines a syntax parser for Veil to use when compiling templates
    /// </summary>
    public interface ITemplateParser
    {
        /// <summary>
        /// Parses the supplied template to a veil syntax tree
        /// </summary>
        /// <param name="templateReader">The contents of the template to be parsed</param>
        /// <param name="modelType">The type of the model that will be passed to the template</param>
        SyntaxTreeNode Parse(TextReader templateReader, Type modelType);
    }
}