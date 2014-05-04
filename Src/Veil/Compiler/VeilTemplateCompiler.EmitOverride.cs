using Veil.Parser;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitOverride(SyntaxTreeNode.OverridePointNode node)
        {
            if (!overrideSections.ContainsKey(node.OverrideName))
            {
                if (node.IsRequired) throw new VeilCompilerException("Overrideable section '{0}' is required but not specified".FormatInvariant(node.OverrideName));
                if (node.DefaultContent != null) EmitNode(node.DefaultContent);
                return;
            }

            var o = overrideSections[node.OverrideName];
            EmitNode(o);
        }
    }
}