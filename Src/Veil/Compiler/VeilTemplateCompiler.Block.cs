using System.Linq;
using System.Linq.Expressions;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleBlock(BlockNode block)
        {
            if (!block.Nodes.Any())
            {
                return Expression.Empty();
            }

            var blockNodes = (from node in block.Nodes
                              select this.HandleNode(node)).ToArray();
            if (blockNodes.Length == 1)
            {
                return blockNodes[0];
            }

            return Expression.Block(blockNodes);
        }
    }
}