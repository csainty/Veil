using System;
using System.Collections;
using System.Reflection;

namespace Veil.Parser.Nodes
{
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

    public class LateBoundNode : ExpressionNode
    {
        public string ItemName { get; set; }

        public override Type ResultType
        {
            get { return typeof(object); }
        }
    }
}