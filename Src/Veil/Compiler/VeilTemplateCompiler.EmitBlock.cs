using Veil.Parser;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitBlock(SyntaxTreeNode.BlockNode node)
        {
            foreach (var n in node.Nodes)
            {
                EmitNode(n);
            }
        }
    }
}