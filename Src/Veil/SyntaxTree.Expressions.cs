using System;
using System.Reflection;

namespace Veil
{
    public interface IModelExpressionNode : ISyntaxTreeNode
    {
        Type Type { get; }
    }

    public class ModelPropertyExpressionNode : IModelExpressionNode
    {
        public PropertyInfo Property { get; set; }

        public Type Type
        {
            get { return this.Property.PropertyType; }
        }

        public static ModelPropertyExpressionNode Create(Type type, string propertyName)
        {
            return new ModelPropertyExpressionNode { Property = type.GetProperty(propertyName) };
        }
    }

    public class ModelFieldExpressionNode : IModelExpressionNode
    {
        public FieldInfo Field { get; set; }

        public Type Type
        {
            get { return this.Field.FieldType; }
        }

        public static ModelFieldExpressionNode Create(Type type, string fieldName)
        {
            return new ModelFieldExpressionNode { Field = type.GetField(fieldName) };
        }
    }

    public class SubModelExpressionNode : IModelExpressionNode
    {
        public IModelExpressionNode ModelExpression { get; set; }

        public IModelExpressionNode SubModelExpression { get; set; }

        public Type Type
        {
            get { return SubModelExpression.Type; }
        }

        public static SubModelExpressionNode Create(IModelExpressionNode modelExpression, IModelExpressionNode subModelExpression)
        {
            return new SubModelExpressionNode
            {
                ModelExpression = modelExpression,
                SubModelExpression = subModelExpression
            };
        }
    }

    public class FunctionCallExpressionNode : IModelExpressionNode
    {
        public MethodInfo Function { get; set; }

        public Type Type
        {
            get { return this.Function.ReturnType; }
        }

        public static FunctionCallExpressionNode Create(Type modelType, string functionName)
        {
            return new FunctionCallExpressionNode { Function = modelType.GetMethod(functionName) };
        }
    }

    public class SelfExpressionNode : IModelExpressionNode
    {
        public Type Type { get; set; }

        public static SelfExpressionNode Create(Type modelType)
        {
            return new SelfExpressionNode { Type = modelType };
        }
    }
}