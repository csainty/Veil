using System.Linq.Expressions;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression Override(Parser.Nodes.OverridePointNode node)
        {
            if (!this.overrideSections.ContainsKey(node.OverrideName))
            {
                if (node.IsRequired) throw new VeilCompilerException("Overrideable section '{0}' is required but not specified".FormatInvariant(node.OverrideName));
                if (node.DefaultContent != null) return Node(node.DefaultContent);
                return Expression.Empty();
            }

            var o = this.overrideSections[node.OverrideName];
            return Node(o);
        }
    }
}