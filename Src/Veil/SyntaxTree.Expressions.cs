using System;
using System.Reflection;

namespace Veil
{
    public abstract partial class SyntaxTreeNode
    {
        public abstract class ExpressionNode : SyntaxTreeNode
        {
            public abstract Type ResultType { get; }

            /// <summary>
            /// Call the getter on the specified property
            /// </summary>
            /// <param name="modelType">The type of the scoped model</param>
            /// <param name="propertyName">The name of the property</param>
            public static ModelPropertyExpressionNode ModelProperty(Type modelType, string propertyName)
            {
                return new ModelPropertyExpressionNode
                {
                    Property = modelType.GetProperty(propertyName)
                };
            }

            /// <summary>
            /// Get a field
            /// </summary>
            /// <param name="modelType">The type of the scoped model</param>
            /// <param name="fieldName">The name of the field</param>
            public static ModelFieldExpressionNode ModelField(Type modelType, string fieldName)
            {
                return new ModelFieldExpressionNode
                {
                    Field = modelType.GetField(fieldName)
                };
            }

            /// <summary>
            /// Traverse one level down a model structure
            /// </summary>
            /// <param name="modelExpression">An expression referencing the model to traverse to</param>
            /// <param name="subModelExpression">An expression to evaluate in the scope of the model that has been traversed to</param>
            public static SubModelExpressionNode ModelSubModel(ExpressionNode modelExpression, ExpressionNode subModelExpression)
            {
                return new SubModelExpressionNode
                {
                    ModelExpression = modelExpression,
                    SubModelExpression = subModelExpression
                };
            }

            /// <summary>
            /// Execute a function
            /// </summary>
            /// <param name="modelType">The type of the scoped model</param>
            /// <param name="functionName">The name of the function</param>
            public static FunctionCallExpressionNode ModelFunction(Type modelType, string functionName)
            {
                return new FunctionCallExpressionNode
                {
                    Function = modelType.GetMethod(functionName)
                };
            }

            /// <summary>
            /// Evaluate the model itself e.g. Value types
            /// </summary>
            /// <param name="modelType">The type of the scoped model</param>
            public static SelfExpressionNode Self(Type modelType)
            {
                return new SelfExpressionNode
                {
                    ModelType = modelType
                };
            }

            public class ModelPropertyExpressionNode : ExpressionNode
            {
                public PropertyInfo Property { get; set; }

                public override Type ResultType
                {
                    get { return this.Property.PropertyType; }
                }
            }

            public class ModelFieldExpressionNode : ExpressionNode
            {
                public FieldInfo Field { get; set; }

                public override Type ResultType
                {
                    get { return this.Field.FieldType; }
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
                public MethodInfo Function { get; set; }

                public override Type ResultType
                {
                    get { return this.Function.ReturnType; }
                }
            }

            public class SelfExpressionNode : ExpressionNode
            {
                public Type ModelType { get; set; }

                public override Type ResultType { get { return this.ModelType; } }
            }
        }
    }
}