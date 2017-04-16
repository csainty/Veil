namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that writes a literal string expression to the <see cref="System.IO.TextWriter"/>
    /// </summary>
    public class WriteLiteralNode : SyntaxTreeNode
    {
        /// <summary>
        /// The content to write to the output
        /// </summary>
        public string LiteralContent { get; set; }
    }
}