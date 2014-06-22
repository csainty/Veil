namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that wraps a block with a new model in scope
    /// </summary>
    public class ScopedBlockNode : SyntaxTreeNode
    {
        public ExpressionNode ModelToScope { get; set; }

        public BlockNode Block { get; set; }
    }
}