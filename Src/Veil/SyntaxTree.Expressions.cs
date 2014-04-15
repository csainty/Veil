using System;
using System.Reflection;

namespace Veil
{
    public abstract partial class SyntaxTreeNode
    {
        public abstract class ExpressionNode : SyntaxTreeNode
        {
            public abstract Type ResultType { get; }

            public static ModelPropertyExpressionNode ModelProperty(Type modelType, string propertyName)
            {
                return new ModelPropertyExpressionNode
                {
                    Property = modelType.GetProperty(propertyName)
                };
            }

            public static ModelFieldExpressionNode ModelField(Type modelType, string fieldName)
            {
                return new ModelFieldExpressionNode
                {
                    Field = modelType.GetField(fieldName)
                };
            }

            public static SubModelExpressionNode ModelSubModel(ExpressionNode modelExpression, ExpressionNode subModelExpression)
            {
                return new SubModelExpressionNode
                {
                    ModelExpression = modelExpression,
                    SubModelExpression = subModelExpression
                };
            }

            public static FunctionCallExpressionNode ModelFunction(Type modelType, string functionName)
            {
                return new FunctionCallExpressionNode
                {
                    Function = modelType.GetMethod(functionName)
                };
            }

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