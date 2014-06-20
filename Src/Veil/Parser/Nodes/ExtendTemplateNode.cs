using System.Collections.Generic;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node which chooser a parent template and loads sections of content in to the parent
    /// </summary>
    public class ExtendTemplateNode : SyntaxTreeNode
    {
        public string TemplateName { get; set; }

        public IDictionary<string, SyntaxTreeNode> Overrides { get; set; }
    }
}