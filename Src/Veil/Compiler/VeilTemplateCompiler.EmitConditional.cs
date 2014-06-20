using System.Linq;
using System.Reflection;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static MethodInfo getTypeMethod = typeof(object).GetMethod("GetType");
        private static MethodInfo boolifyMethod = typeof(Helpers).GetMethod("Boolify");

        private void EmitConditional(ConditionalNode node)
        {
            var hasTrueBlock = node.TrueBlock != null && node.TrueBlock.Nodes.Any();
            var hasFalseBlock = node.FalseBlock != null && node.FalseBlock.Nodes.Any();

            if (!hasTrueBlock && !hasFalseBlock)
            {
                throw new VeilCompilerException("Conditionals must have a True or False block");
            }

            var done = emitter.DefineLabel();
            EvaluateExpression(node.Expression);

            if (node.Expression.ResultType == typeof(object))
            {
                emitter.CallMethod(boolifyMethod);
            }

            if (!hasTrueBlock)
            {
                emitter.BranchIfTrue(done);
                EmitNode(node.FalseBlock);
            }
            else if (!hasFalseBlock)
            {
                emitter.BranchIfFalse(done);
                EmitNode(node.TrueBlock);
            }
            else
            {
                var falseBlock = emitter.DefineLabel();

                emitter.BranchIfFalse(falseBlock);
                EmitNode(node.TrueBlock);
                emitter.Branch(done);

                emitter.MarkLabel(falseBlock);
                EmitNode(node.FalseBlock);
            }

            emitter.MarkLabel(done);
        }
    }
}