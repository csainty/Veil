namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node repsenting a section in a Master/Parent page that can be replaced when the page is extended
    /// </summary>
    public class OverridePointNode : SyntaxTreeNode
    {
        public string OverrideName { get; set; }

        public bool IsRequired { get; set; }

        public SyntaxTreeNode DefaultContent { get; set; }
    }
}