using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitWriteLiteral(WriteLiteralNode node)
        {
            if (string.IsNullOrEmpty(node.LiteralContent)) return;

            LoadWriterToStack();
            emitter.LoadConstant(node.LiteralContent);
            CallWriteFor(typeof(string));
        }
    }
}