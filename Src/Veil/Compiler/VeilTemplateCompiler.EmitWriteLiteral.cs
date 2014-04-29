namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitWriteLiteral(SyntaxTreeNode.WriteLiteralNode node)
        {
            if (node.LiteralType == typeof(string))
            {
                emitter.OutputLiteral((string)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(int))
            {
                emitter.OutputLiteral((int)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(double))
            {
                emitter.OutputLiteral((double)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(float))
            {
                emitter.OutputLiteral((float)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(long))
            {
                emitter.OutputLiteral((long)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(uint))
            {
                emitter.OutputLiteral((uint)node.LiteralContent);
            }
            else if (node.LiteralType == typeof(ulong))
            {
                emitter.OutputLiteral((ulong)node.LiteralContent);
            }
            else
            {
                throw new VeilCompilerException("Unable to write literal of type {0}".FormatInvariant(node.LiteralType.Name));
            }
        }
    }
}