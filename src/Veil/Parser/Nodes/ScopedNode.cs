namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that wraps another node with a new model in scope
    /// </summary>
    public class ScopedNode : SyntaxTreeNode
    {
        /// <summary>
        /// An expression which evaluates to the model that is to be in scope
        /// </summary>
        public ExpressionNode ModelToScope { get; set; }

        /// <summary>
        /// The node to execute in the specified scope
        /// </summary>
        public SyntaxTreeNode Node { get; set; }
    }
}