using System.Linq;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitConditional<T>(VeilCompilerState<T> state, SyntaxTreeNode.ConditionalNode node)
        {
            var hasTrueBlock = node.TrueBlock != null && node.TrueBlock.Nodes.Any();
            var hasFalseBlock = node.FalseBlock != null && node.FalseBlock.Nodes.Any();

            if (!hasTrueBlock && !hasFalseBlock)
            {
                throw new VeilCompilerException("Conditionals must have a True or False block");
            }

            if (!hasTrueBlock)
            {
                var done = state.Emitter.DefineLabel();
                state.PushExpressionScopeOnStack(node.Expression);
                state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
                state.Emitter.BranchIfTrue(done);
                EmitNode(state, node.FalseBlock);
                state.Emitter.MarkLabel(done);
            }
            else if (!hasFalseBlock)
            {
                var done = state.Emitter.DefineLabel();
                state.PushExpressionScopeOnStack(node.Expression);
                state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
                state.Emitter.BranchIfFalse(done);
                EmitNode(state, node.TrueBlock);
                state.Emitter.MarkLabel(done);
            }
            else
            {
                var done = state.Emitter.DefineLabel();
                var falseBlock = state.Emitter.DefineLabel();

                state.PushExpressionScopeOnStack(node.Expression);
                state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
                state.Emitter.BranchIfFalse(falseBlock);
                EmitNode(state, node.TrueBlock);
                state.Emitter.Branch(done);

                state.Emitter.MarkLabel(falseBlock);
                EmitNode(state, node.FalseBlock);

                state.Emitter.MarkLabel(done);
            }
        }
    }
}