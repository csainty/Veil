using Veil.Parser;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitWriteLiteral(SyntaxTreeNode.WriteLiteralNode node)
        {
            LoadWriterToStack();
            emitter.LoadConstant(node.LiteralContent);
            CallWriteFor(typeof(string));
        }
    }
}