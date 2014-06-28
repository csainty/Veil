namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node repsenting a section in a Master/Parent page that can be replaced when the page is extended
    /// </summary>
    public class OverridePointNode : SyntaxTreeNode
    {
        /// <summary>
        /// The name of the override to be replaced from <see cref="ExtendTemplateNode.Overrides"/>
        /// </summary>
        public string OverrideName { get; set; }

        /// <summary>
        /// Indicates whether the override is required for the template to parse
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// The content to render when an optional override is not overridden
        /// </summary>
        public SyntaxTreeNode DefaultContent { get; set; }
    }
}