using System;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates whether the referenced collection has any items
    /// </summary>
    public class CollectionHasItemsExpressionNode : ExpressionNode
    {
        private ExpressionNode collectionExpression;

        /// <summary>
        /// An expression that evaluates to an <see cref="System.Collections.ICollection"/> which is checked for items
        /// </summary>
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
            if (!this.collectionExpression.ResultType.HasCollectionInterface()) throw new VeilParserException("Expression assigned to CollectionHasItemsNode.CollectionExpression is not an ICollection");
        }

        /// <summary>
        /// The type of result from this expression evaluation
        /// </summary>
        public override Type ResultType { get { return typeof(bool); } }
    }
}