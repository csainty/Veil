using System;
using System.Collections;
using System.Reflection;

namespace Veil.Parser
{
    public abstract partial class SyntaxTreeNode
    {
        public enum ExpressionScope
        {
            CurrentModelOnStack,
            RootModel
        }

        public abstract class ExpressionNode : SyntaxTreeNode
        {
            public ExpressionScope Scope { get; set; }

            public abstract Type ResultType { get; }

            /// <summary>
            /// Call the getter on the specified property
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
            /// Get a field
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
            /// Traverse one level down a model structure
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
            /// Execute a function
            /// </summary>
            /// <param name="modelType">The type of the scoped model</param>
            /// <param name="functionName">The name of the function</param>
            /// <param name="scope">The scope this expression evaluated in</param>
            public static FunctionCallExpressionNode Function(Type modelType, string functionName, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
            {
                return new FunctionCallExpressionNode
                {
                    MethodInfo = modelType.GetMethod(functionName),
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
            /// </summary>
            /// <param name="collectionExpression">An expression referencing a Collection</param>
            public static CollectionHasItemsNode HasItems(ExpressionNode collectionExpression)
            {
                return new CollectionHasItemsNode
                {
                    CollectionExpression = collectionExpression,
                    Scope = collectionExpression.Scope
                };
            }

            public class PropertyExpressionNode : ExpressionNode
            {
                public PropertyInfo PropertyInfo { get; set; }

                public override Type ResultType
                {
                    get { return this.PropertyInfo.PropertyType; }
                }
            }

            public class FieldExpressionNode : ExpressionNode
            {
                public FieldInfo FieldInfo { get; set; }

                public override Type ResultType
                {
                    get { return this.FieldInfo.FieldType; }
                }
            }

            public class SubModelExpressionNode : ExpressionNode
            {
                public ExpressionNode ModelExpression { get; set; }

                public ExpressionNode SubModelExpression { get; set; }

                public override Type ResultType
                {
                    get { return SubModelExpression.ResultType; }
                }
            }

            public class FunctionCallExpressionNode : ExpressionNode
            {
                public MethodInfo MethodInfo { get; set; }

                public override Type ResultType
                {
                    get { return this.MethodInfo.ReturnType; }
                }
            }

            public class SelfExpressionNode : ExpressionNode
            {
                public Type ModelType { get; set; }

                public override Type ResultType { get { return this.ModelType; } }
            }

            public class CollectionHasItemsNode : ExpressionNode
            {
                private ExpressionNode collectionExpression;

                public ExpressionNode CollectionExpression
                {
                    get
                    {
                        return this.collectionExpression;
                    }
                    set
                    {
                        this.collectionExpression = value;
                        this.Validate();
                    }
                }

                private void Validate()
                {
                    if (this.collectionExpression == null) throw new ArgumentNullException("CollectionExpression");
                    if (!typeof(ICollection).IsAssignableFrom(this.collectionExpression.ResultType)) throw new VeilParserException("Expression assigned to CollectionHasItemsNode.CollectionExpression is not an ICollection");
                }

                public override Type ResultType { get { return typeof(bool); } }
            }
        }
    }
}