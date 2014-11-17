using System.Linq.Expressions;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression Override(OverridePointNode node)
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