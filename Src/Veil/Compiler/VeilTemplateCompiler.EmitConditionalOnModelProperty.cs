using System.Linq;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitConditionalOnModelProperty<T>(VeilCompilerState<T> state, ConditionalOnModelExpressionNode node)
        {
            if (node.TrueBlock == null || !node.TrueBlock.Nodes.Any())
            {
                throw new VeilCompilerException("Conditionals must have a True block");
            }

            if (node.FalseBlock == null || !node.FalseBlock.Nodes.Any())
            {
                var done = state.Emitter.DefineLabel();
                state.PushCurrentModelOnStack();
                state.Emitter.LoadExpressionFromCurrentModelOnStack(node.Expression);
                state.Emitter.BranchIfFalse(done);
                EmitNode(state, node.TrueBlock);
                state.Emitter.MarkLabel(done);
            }
            else
            {
                var done = state.Emitter.DefineLabel();
                var falseBlock = state.Emitter.DefineLabel();

                state.PushCurrentModelOnStack();
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