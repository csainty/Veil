using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression Conditional(Parser.Nodes.ConditionalNode node)
        {
            var hasTrueBlock = node.TrueBlock != null && node.TrueBlock.Nodes.Any();
            var hasFalseBlock = node.FalseBlock != null && node.FalseBlock.Nodes.Any();

            if (!hasTrueBlock && !hasFalseBlock)
            {
                throw new VeilCompilerException("Conditionals must have a True or False block");
            }

            var value = ParseExpression(node.Expression);
            var check = BoolifyExpression(ref value);

            if (!hasFalseBlock)
            {
                return Expression.IfThen(check, Node(node.TrueBlock));
            }
            else if (!hasTrueBlock)
            {
                return Expression.IfThen(Expression.IsFalse(check), Node(node.FalseBlock));
            }
            return Expression.IfThenElse(check, Node(node.TrueBlock), Node(node.FalseBlock));
        }

        private static readonly MethodInfo boolify = typeof(Helpers).GetMethod("Boolify");

        private static Expression BoolifyExpression(ref TypedExpression expression)
        {
            if (expression.Type == typeof(bool))
            {
                return expression.Expression;
            }
            return Expression.Call(null, boolify, expression.Expression);
        }
    }
}