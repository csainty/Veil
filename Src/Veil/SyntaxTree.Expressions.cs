using System;
using System.Reflection;

namespace Veil
{
    public abstract class ModelExpressionNode : SyntaxTreeNode
    {
        public abstract Type Type { get; }
    }

    public class ModelPropertyExpressionNode : ModelExpressionNode
    {
        public PropertyInfo Property { get; set; }

        public override Type Type
        {
            get { return this.Property.PropertyType; }
        }

        public static ModelPropertyExpressionNode Create(Type type, string propertyName)
        {
            return new ModelPropertyExpressionNode { Property = type.GetProperty(propertyName) };
        }
    }

    public class ModelFieldExpressionNode : ModelExpressionNode
    {
        public FieldInfo Field { get; set; }

        public override Type Type
        {
            get { return this.Field.FieldType; }
        }

        public static ModelFieldExpressionNode Create(Type type, string fieldName)
        {
            return new ModelFieldExpressionNode { Field = type.GetField(fieldName) };
        }
    }

    public class SubModelExpressionNode : ModelExpressionNode
    {
        public ModelExpressionNode ModelExpression { get; set; }

        public ModelExpressionNode SubModelExpression { get; set; }

        public override Type Type
        {
            get { return SubModelExpression.Type; }
        }

        public static SubModelExpressionNode Create(ModelExpressionNode modelExpression, ModelExpressionNode subModelExpression)
        {
            return new SubModelExpressionNode
            {
                ModelExpression = modelExpression,
                SubModelExpression = subModelExpression
            };
        }
    }

    public class FunctionCallExpressionNode : ModelExpressionNode
    {
        public MethodInfo Function { get; set; }

        public override Type Type
        {
            get { return this.Function.ReturnType; }
        }

        public static FunctionCallExpressionNode Create(Type modelType, string functionName)
        {
            return new FunctionCallExpressionNode { Function = modelType.GetMethod(functionName) };
        }
    }

    public class SelfExpressionNode : ModelExpressionNode
    {
        private Type type;

        public override Type Type { get { return this.type; } }

        public static SelfExpressionNode Create(Type modelType)
        {
            return new SelfExpressionNode { type = modelType };
        }
    }
}