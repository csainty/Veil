using System.Linq.Expressions;
using System.Reflection;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo runtimeBindFunction = typeof(Helpers).GetMethod("RuntimeBind");

        private Expression ParseExpression(ExpressionNode node)
        {
            if (node is PropertyExpressionNode) return EvaluateProperty((PropertyExpressionNode)node);
            if (node is FieldExpressionNode) return EvaluateField((FieldExpressionNode)node);
            if (node is SubModelExpressionNode) return EvaluateSubModel((SubModelExpressionNode)node);
            if (node is SelfExpressionNode) return EvaluateSelfExpressionNode((SelfExpressionNode)node);
            if (node is LateBoundExpressionNode) return EvaluateLateBoundExpression((LateBoundExpressionNode)node);
            if (node is CollectionHasItemsExpressionNode) return EvaluateHasItemsNode((CollectionHasItemsExpressionNode)node);
            if (node is FunctionCallExpressionNode) return EvaluateFunctionCall((FunctionCallExpressionNode)node);

            throw new VeilCompilerException("Unknown expression type '{0}'".FormatInvariant(node.GetType().Name));
        }

        private Expression EvaluateFunctionCall(FunctionCallExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            return Expression.Call(model, node.MethodInfo);
        }

        private Expression EvaluateHasItemsNode(CollectionHasItemsExpressionNode node)
        {
            var collection = this.ParseExpression(node.CollectionExpression);
            var count = node.CollectionExpression.ResultType.GetCollectionInterface().GetProperty("Count");
            return Expression.NotEqual(Expression.Property(collection, count), Expression.Constant(0));
        }

        private Expression EvaluateLateBoundExpression(LateBoundExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            return Expression.Call(null, runtimeBindFunction, new[] {
                model,
                Expression.Constant(node.ItemName),
                Expression.Constant(node.IsCaseSensitive)
            });
        }

        private Expression EvaluateSelfExpressionNode(SelfExpressionNode node)
        {
            return EvaluateScope(node.Scope);
        }

        private Expression EvaluateSubModel(SubModelExpressionNode node)
        {
            var model = ParseExpression(node.ModelExpression);
            PushScope(model);
            var subModel = ParseExpression(node.SubModelExpression);
            PopScope();
            return subModel;
        }

        private Expression EvaluateField(FieldExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            return Expression.Field(model, node.FieldInfo);
        }

        private Expression EvaluateProperty(PropertyExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            return Expression.Property(model, node.PropertyInfo);
        }

        private Expression EvaluateScope(ExpressionScope scope)
        {
            switch (scope)
            {
                case ExpressionScope.CurrentModelOnStack: return this.modelStack.First.Value;
                case ExpressionScope.RootModel: return this.modelStack.Last.Value;
                case ExpressionScope.ModelOfParentScope: return this.modelStack.First.Next.Value;
                default:
                    throw new VeilCompilerException("Unknown expression scope '{0}'".FormatInvariant(scope));
            }
        }
    }
}