using System;
using Veil.Parser.Nodes;

namespace Veil.Parser
{
    /// <summary>
    /// Factory methods for create expression nodes
    /// </summary>
    public static class Expression
    {
        /// <summary>
        /// Evaluate a property on the model object
        /// </summary>
        /// <param name="modelType">The type of the scoped model</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="scope">The scope this expression evaluated in</param>
        public static PropertyExpressionNode Property(Type modelType, string propertyName, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
        {
            return new PropertyExpressionNode
            {
                PropertyInfo = modelType.GetProperty(propertyName),
                Scope = scope
            };
        }

        /// <summary>
        /// Evaluate a field on the model object
        /// </summary>
        /// <param name="modelType">The type of the scoped model</param>
        /// <param name="fieldName">The name of the field</param>
        /// <param name="scope">The scope this expression evaluated in</param>
        public static FieldExpressionNode Field(Type modelType, string fieldName, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
        {
            return new FieldExpressionNode
            {
                FieldInfo = modelType.GetField(fieldName),
                Scope = scope
            };
        }

        /// <summary>
        /// Evaluate an expression on a sub model, can be nested to traverse any depth of sub models
        /// </summary>
        /// <param name="modelExpression">An expression referencing the model to traverse to</param>
        /// <param name="subModelExpression">An expression to evaluate in the scope of the model that has been traversed to</param>
        /// <param name="scope">The scope this expression evaluated in</param>
        public static SubModelExpressionNode SubModel(ExpressionNode modelExpression, ExpressionNode subModelExpression, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
        {
            return new SubModelExpressionNode
            {
                ModelExpression = modelExpression,
                SubModelExpression = subModelExpression,
                Scope = scope
            };
        }

        /// <summary>
        /// Evaluate a function call on the model
        /// </summary>
        /// <param name="modelType">The type of the scoped model</param>
        /// <param name="functionName">The name of the function</param>
        /// <param name="scope">The scope this expression evaluated in</param>
        public static FunctionCallExpressionNode Function(Type modelType, string functionName, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
        {
            return new FunctionCallExpressionNode
            {
                MethodInfo = modelType.GetMethod(functionName, new Type[0]),
                Scope = scope
            };
        }

        /// <summary>
        /// Evaluate the model itself e.g. Value types
        /// </summary>
        /// <param name="modelType">The type of the scoped model</param>
        /// <param name="scope">The scope this expression evaluated in</param>
        public static SelfExpressionNode Self(Type modelType, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
        {
            return new SelfExpressionNode
            {
                ModelType = modelType,
                Scope = scope
            };
        }

        /// <summary>
        /// Evaluate whether the collectionExpression has Count > 0
        /// Can only be used on types that implement <see cref="System.Collections.ICollection"/>
        /// </summary>
        /// <param name="collectionExpression">An expression referencing a Collection</param>
        public static CollectionHasItemsExpressionNode HasItems(ExpressionNode collectionExpression)
        {
            return new CollectionHasItemsExpressionNode
            {
                CollectionExpression = collectionExpression,
                Scope = collectionExpression.Scope
            };
        }

        /// <summary>
        /// Evaluate a property at runtime against an unknown model type
        /// </summary>
        /// <param name="itemName">The name of the proeprty that will be searched for</param>
        /// <param name="isCaseSenstiive">Indcates whether the expression should be evaluated with case sensitivity</param>
        /// <param name="scope">The scope this expression evaluated in</param>
        public static LateBoundExpressionNode LateBound(string itemName, bool isCaseSenstiive = true, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
        {
            return new LateBoundExpressionNode
            {
                ItemName = itemName,
                Scope = scope
            };
        }
    }
}