using Veil.Parser;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitWriteLiteral(SyntaxTreeNode.WriteLiteralNode node)
        {
            LoadWriterToStack();

            if (node.LiteralType == typeof(string)) emitter.LoadConstant((string)node.LiteralContent);
            else if (node.LiteralType == typeof(int)) emitter.LoadConstant((int)node.LiteralContent);
            else if (node.LiteralType == typeof(double)) emitter.LoadConstant((double)node.LiteralContent);
            else if (node.LiteralType == typeof(float)) emitter.LoadConstant((float)node.LiteralContent);
            else if (node.LiteralType == typeof(long)) emitter.LoadConstant((long)node.LiteralContent);
            else if (node.LiteralType == typeof(uint)) emitter.LoadConstant((uint)node.LiteralContent);
            else if (node.LiteralType == typeof(ulong)) emitter.LoadConstant((ulong)node.LiteralContent);
            else throw new VeilCompilerException("Unable to write literal of type {0}".FormatInvariant(node.LiteralType.Name));

            CallWriteFor(node.LiteralType);
        }
    }
}