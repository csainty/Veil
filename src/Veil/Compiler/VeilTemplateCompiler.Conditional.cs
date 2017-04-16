using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private Expression HandleConditional(ConditionalNode node)
        {
            var hasTrueBlock = node.TrueBlock != null && node.TrueBlock.Nodes.Any();
            var hasFalseBlock = node.FalseBlock != null && node.FalseBlock.Nodes.Any();

            if (!hasTrueBlock && !hasFalseBlock)
            {
                throw new VeilCompilerException("Conditionals must have a True or False block");
            }

            var valueToCheck = ParseExpression(node.Expression);
            var booleanCheck = BoolifyExpression(valueToCheck);

            if (!hasFalseBlock)
            {
                return Expression.IfThen(booleanCheck, HandleNode(node.TrueBlock));
            }
            else if (!hasTrueBlock)
            {
                return Expression.IfThen(Expression.IsFalse(booleanCheck), HandleNode(node.FalseBlock));
            }
            return Expression.IfThenElse(booleanCheck, HandleNode(node.TrueBlock), HandleNode(node.FalseBlock));
        }

        private static readonly MethodInfo boolify = typeof(Helpers).GetMethod("Boolify");

        private static Expression BoolifyExpression(Expression expression)
        {
            if (expression.Type == typeof(bool))
            {
                return expression;
            }
            return Expression.Call(null, boolify, expression);
        }
    }
}