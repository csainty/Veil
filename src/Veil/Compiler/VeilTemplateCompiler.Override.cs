using System.Linq.Expressions;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleOverride(OverridePointNode node)
        {
            if (!this.overrideSections.ContainsKey(node.OverrideName))
            {
                if (node.IsRequired) throw new VeilCompilerException("Overrideable section '{0}' is required but not specified".FormatInvariant(node.OverrideName));
                if (node.DefaultContent != null) return HandleNode(node.DefaultContent);
                return Expression.Empty();
            }

            var o = this.overrideSections[node.OverrideName];
            return HandleNode(o);
        }
    }
}