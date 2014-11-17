using System.Linq;
using System.Linq.Expressions;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression Block(Parser.Nodes.BlockNode block)
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