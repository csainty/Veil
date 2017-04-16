using System.Collections.Generic;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node which chooser a parent template and loads sections of content in to the parent
    /// </summary>
    public class ExtendTemplateNode : SyntaxTreeNode
    {
        /// <summary>
        /// The name of the template to load via <see cref="IVeilContext.GetTemplateByName"/>
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// A set of overrides to insert in to the template
        /// </summary>
        public IDictionary<string, SyntaxTreeNode> Overrides { get; set; }
    }
}