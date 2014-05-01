using System.Linq;
using Veil.Parser;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitConditional(SyntaxTreeNode.ConditionalNode node)
        {
            var hasTrueBlock = node.TrueBlock != null && node.TrueBlock.Nodes.Any();
            var hasFalseBlock = node.FalseBlock != null && node.FalseBlock.Nodes.Any();

            if (!hasTrueBlock && !hasFalseBlock)
            {
                throw new VeilCompilerException("Conditionals must have a True or False block");
            }

            if (!hasTrueBlock)
            {
                var done = emitter.DefineLabel();
                PushExpressionScopeOnStack(node.Expression);
                emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
                emitter.BranchIfTrue(done);
                EmitNode(node.FalseBlock);
                emitter.MarkLabel(done);
            }
            else if (!hasFalseBlock)
            {
                var done = emitter.DefineLabel();
                PushExpressionScopeOnStack(node.Expression);
                emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
                emitter.BranchIfFalse(done);
                EmitNode(node.TrueBlock);
                emitter.MarkLabel(done);
            }
            else
            {
                var done = emitter.DefineLabel();
                var falseBlock = emitter.DefineLabel();

                PushExpressionScopeOnStack(node.Expression);
                emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
                emitter.BranchIfFalse(falseBlock);
                EmitNode(node.TrueBlock);
                emitter.Branch(done);

                emitter.MarkLabel(falseBlock);
                EmitNode(node.FalseBlock);

                emitter.MarkLabel(done);
            }
        }
    }
}