using System;
using System.Reflection;

namespace Veil
{
    internal interface IModelExpressionNode : ISyntaxTreeNode
    {
        Type Type { get; }
    }

    internal class ModelPropertyExpressionNode : IModelExpressionNode
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

    internal class ModelFieldExpressionNode : IModelExpressionNode
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

    internal class SubModelExpressionNode : IModelExpressionNode
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

    internal class FunctionCallExpressionNode : IModelExpressionNode
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

    internal class SelfExpressionNode : IModelExpressionNode
    {
        public Type Type { get; set; }

        public static SelfExpressionNode Create(Type modelType)
        {
            return new SelfExpressionNode { Type = modelType };
        }
    }
}