using System.Collections.Generic;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that contains a sequence of other nodes
    /// </summary>
    public class BlockNode : SyntaxTreeNode
    {
        private List<SyntaxTreeNode> nodes = new List<SyntaxTreeNode>();

        /// <summary>
        /// Get all the nodes currently in the block
        /// </summary>
        public IEnumerable<SyntaxTreeNode> Nodes { get { return this.nodes; } }

        /// <summary>
        /// Adds a new node to the block
        /// </summary>
        /// <param name="node"></param>
        public void Add(SyntaxTreeNode node)
        {
            this.nodes.Add(node);
        }

        /// <summary>
        /// Adds a set of nodes to the block
        /// </summary>
        public void AddRange(IEnumerable<SyntaxTreeNode> nodes)
        {
            this.nodes.AddRange(nodes);
        }

        /// <summary>
        /// Determines if there are any nodes in the block already
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return this.nodes.Count == 0;
        }

        /// <summary>
        /// Returns the last node in the block
        /// </summary>
        /// <returns></returns>
        public SyntaxTreeNode LastNode()
        {
            return this.nodes[this.nodes.Count - 1];
        }
    }
}