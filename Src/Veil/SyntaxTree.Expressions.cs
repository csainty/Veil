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
}