namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node which triggers a Flush() on the <see cref="TextWriter"/> to allow early return of content over the wire
    /// </summary>
    public class FlushNode : SyntaxTreeNode
    {
    }
}