namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that writes a literal string expression to the <see cref="TextWriter"/>
    /// </summary>
    public class WriteLiteralNode : SyntaxTreeNode
    {
        public string LiteralContent { get; set; }
    }
}