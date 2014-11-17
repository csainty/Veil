using System.Linq;
using System.Linq.Expressions;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression Block(BlockNode block)
        {
            if (!block.Nodes.Any())
            {
                return Expression.Empty();
            }

            var nodes = from node in block.Nodes
                        select Node(node);
            return Expression.Block(nodes);
        }
    }
}