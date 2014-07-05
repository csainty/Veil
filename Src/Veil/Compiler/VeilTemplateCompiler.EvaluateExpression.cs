using System.Reflection;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo runtimeBindMethod = typeof(Helpers).GetMethod("RuntimeBind");

        private void EvaluateExpression(ExpressionNode expression)
        {
            switch (expression.Scope)
            {
                case ExpressionScope.CurrentModelOnStack:
                    scopeStack.First.Value.Invoke(emitter);
                    break;

                case ExpressionScope.RootModel:
                    scopeStack.Last.Value.Invoke(emitter);
                    break;

                case ExpressionScope.ModelOfParentScope:
                    scopeStack.First.Next.Value.Invoke(emitter);
                    break;

                default:
                    throw new VeilCompilerException("Unknown expression scope '{0}'".FormatInvariant(expression.Scope));
            }
            EvaluateExpressionAgainstModelOnStack(expression);
        }

        private void EvaluateExpressionAgainstModelOnStack(ExpressionNode expression)
        {
            if (expression is PropertyExpressionNode)
            {
                EvaluatePropertyExpression((PropertyExpressionNode)expression);
            }
            else if (expression is FieldExpressionNode)
            {
                EvaluateFieldExpression((FieldExpressionNode)expression);
            }
            else if (expression is SubModelExpressionNode)
            {
                EvaluateSubModelExpression((SubModelExpressionNode)expression);
            }
            else if (expression is FunctionCallExpressionNode)
            {
                EvaluateFunctionCallExpressionNode((FunctionCallExpressionNode)expression);
            }
            else if (expression is CollectionHasItemsExpressionNode)
            {
                EvaluateCollectionHasItemsExpressionNode((CollectionHasItemsExpressionNode)expression);
            }
            else if (expression is LateBoundExpressionNode)
            {
                EvaluateLateBoundExpression((LateBoundExpressionNode)expression);
            }
            else if (expression is SelfExpressionNode)
            {
            }
            else
            {
                throw new VeilCompilerException("Unknown expression type '{0}'".FormatInvariant(expression.GetType().Name));
            }
        }

        private void EvaluatePropertyExpression(PropertyExpressionNode node)
        {
            emitter.CallMethod(node.PropertyInfo.GetGetMethod());
        }

        private void EvaluateFieldExpression(FieldExpressionNode node)
        {
            emitter.LoadField(node.FieldInfo);
        }

        private void EvaluateSubModelExpression(SubModelExpressionNode node)
        {
            EvaluateExpressionAgainstModelOnStack(node.ModelExpression);
            EvaluateExpressionAgainstModelOnStack(node.SubModelExpression);
        }

        private void EvaluateFunctionCallExpressionNode(FunctionCallExpressionNode node)
        {
            emitter.CallMethod(node.MethodInfo);
        }

        private void EvaluateCollectionHasItemsExpressionNode(CollectionHasItemsExpressionNode node)
        {
            EvaluateExpressionAgainstModelOnStack(node.CollectionExpression);

            var count = node.CollectionExpression.ResultType.GetCollectionInterface().GetProperty("Count");
            emitter.CallMethod(count.GetGetMethod());
            emitter.LoadConstant(0);
            emitter.CompareEqual();
            emitter.LoadConstant(0);
            emitter.CompareEqual();
        }

        private void EvaluateLateBoundExpression(LateBoundExpressionNode node)
        {
            emitter.LoadConstant(node.ItemName);
            emitter.LoadConstant(node.IsCaseSensitive);
            emitter.CallMethod(runtimeBindMethod);
        }
    }
}