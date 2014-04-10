using System;
using System.IO;
using System.Linq;
using Sigil;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler
    {
        private static void EmitConditionalOnModelProperty<T>(Emit<Action<TextWriter, T>> emitter, ConditionalOnModelExpressionNode node)
        {
            if (node.TrueBlock == null || !node.TrueBlock.Nodes.Any())
            {
                throw new VeilCompilerException("Conditionals must have a True block");
            }

            if (node.FalseBlock == null || !node.FalseBlock.Nodes.Any())
            {
                var done = emitter.DefineLabel();
                emitter.LoadModelExpressionToStack(node.Expression);
                emitter.BranchIfFalse(done);
                EmitNode(emitter, node.TrueBlock);
                emitter.MarkLabel(done);
            }
            else
            {
                var done = emitter.DefineLabel();
                var falseBlock = emitter.DefineLabel();

                emitter.LoadModelExpressionToStack(node.Expression);
                emitter.BranchIfFalse(falseBlock);
                EmitNode(emitter, node.TrueBlock);
                emitter.Branch(done);

                emitter.MarkLabel(falseBlock);
                EmitNode(emitter, node.FalseBlock);

                emitter.MarkLabel(done);
            }
        }
    }
}