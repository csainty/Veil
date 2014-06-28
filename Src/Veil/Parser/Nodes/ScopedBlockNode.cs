namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that wraps a block with a new model in scope
    /// </summary>
    public class ScopedBlockNode : SyntaxTreeNode
    {
        /// <summary>
        /// An expression which evaluates to the model that is to be in scope
        /// </summary>
        public ExpressionNode ModelToScope { get; set; }

        /// <summary>
        /// The block to execute in the specified scope
        /// </summary>
        public BlockNode Block { get; set; }
    }
}