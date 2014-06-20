using System.Collections.Generic;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that contains a sequence of other nodes
    /// </summary>
    public class BlockNode : SyntaxTreeNode
    {
        private List<SyntaxTreeNode> nodes;

        public BlockNode()
        {
            this.nodes = new List<SyntaxTreeNode>();
        }

        public void Add(SyntaxTreeNode node)
        {
            this.nodes.Add(node);
        }

        public void AddRange(IEnumerable<SyntaxTreeNode> nodes)
        {
            this.nodes.AddRange(nodes);
        }

        public IEnumerable<SyntaxTreeNode> Nodes { get { return this.nodes; } }

        public bool IsEmpty()
        {
            return this.nodes.Count == 0;
        }
    }
}