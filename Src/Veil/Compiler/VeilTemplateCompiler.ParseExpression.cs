using System.Linq.Expressions;
using System.Reflection;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static readonly MethodInfo runtimeBindFunction = typeof(Helpers).GetMethod("RuntimeBind");

        private TypedExpression ParseExpression(Parser.ExpressionNode node)
        {
            if (node is Parser.Nodes.PropertyExpressionNode) return EvaluateProperty((Parser.Nodes.PropertyExpressionNode)node);
            if (node is Parser.Nodes.FieldExpressionNode) return EvaluateField((Parser.Nodes.FieldExpressionNode)node);
            if (node is Parser.Nodes.SubModelExpressionNode) return EvaluateSubModel((Parser.Nodes.SubModelExpressionNode)node);
            if (node is Parser.Nodes.SelfExpressionNode) return EvaluateSelfExpressionNode((Parser.Nodes.SelfExpressionNode)node);
            if (node is Parser.Nodes.LateBoundExpressionNode) return EvaluateLateBoundExpression((Parser.Nodes.LateBoundExpressionNode)node);
            if (node is Parser.Nodes.CollectionHasItemsExpressionNode) return EvaluateHasItemsNode((Parser.Nodes.CollectionHasItemsExpressionNode)node);
            if (node is Parser.Nodes.FunctionCallExpressionNode) return EvaluateFunctionCall((Parser.Nodes.FunctionCallExpressionNode)node);

            throw new VeilCompilerException("Unknown expression type '{0}'".FormatInvariant(node.GetType().Name));
        }

        private TypedExpression EvaluateFunctionCall(Parser.Nodes.FunctionCallExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            var call = Expression.Call(model, node.MethodInfo);
            return new TypedExpression(call, node.ResultType);
        }

        private TypedExpression EvaluateHasItemsNode(Parser.Nodes.CollectionHasItemsExpressionNode node)
        {
            var collection = this.ParseExpression(node.CollectionExpression);
            var count = node.CollectionExpression.ResultType.GetCollectionInterface().GetProperty("Count");
            var check = Expression.NotEqual(Expression.Property(collection.Expression, count), Expression.Constant(0));
            return new TypedExpression(check, typeof(bool));
        }

        private TypedExpression EvaluateLateBoundExpression(Parser.Nodes.LateBoundExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            return new TypedExpression(Expression.Call(null, runtimeBindFunction, new[] {
                model,
                Expression.Constant(node.ItemName),
                Expression.Constant(node.IsCaseSensitive)
            }), node.ResultType);
        }

        private TypedExpression EvaluateSelfExpressionNode(Parser.Nodes.SelfExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            return new TypedExpression(model, node.ResultType);
        }

        private TypedExpression EvaluateSubModel(Parser.Nodes.SubModelExpressionNode node)
        {
            var model = ParseExpression(node.ModelExpression);
            PushScope(model.Expression);
            var subModel = ParseExpression(node.SubModelExpression);
            PopScope();
            return subModel;
        }

        private TypedExpression EvaluateField(Parser.Nodes.FieldExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            return new TypedExpression(Expression.Field(model, node.FieldInfo), node.ResultType);
        }

        private TypedExpression EvaluateProperty(Parser.Nodes.PropertyExpressionNode node)
        {
            var model = EvaluateScope(node.Scope);
            return new TypedExpression(Expression.Property(model, node.PropertyInfo), node.ResultType);
        }

        private Expression EvaluateScope(Parser.ExpressionScope scope)
        {
            switch (scope)
            {
                case Parser.ExpressionScope.CurrentModelOnStack: return this.modelStack.First.Value;
                case Parser.ExpressionScope.RootModel: return this.modelStack.Last.Value;
                case Parser.ExpressionScope.ModelOfParentScope: return this.modelStack.First.Next.Value;
                default:
                    throw new VeilCompilerException("Unknown expression scope '{0}'".FormatInvariant(scope));
            }
        }
    }
}