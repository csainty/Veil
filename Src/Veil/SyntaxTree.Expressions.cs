using System;
using System.Reflection;

namespace Veil
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
                    Property = modelType.GetProperty(propertyName),
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
                    Field = modelType.GetField(fieldName),
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
                    Function = modelType.GetMethod(functionName),
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

            public class PropertyExpressionNode : ExpressionNode
            {
                public PropertyInfo Property { get; set; }

                public override Type ResultType
                {
                    get { return this.Property.PropertyType; }
                }
            }

            public class FieldExpressionNode : ExpressionNode
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