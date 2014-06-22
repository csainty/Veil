﻿using System;
using System.Collections;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates whether the referenced collection has any items
    /// </summary>
    public class CollectionHasItemsExpressionNode : ExpressionNode
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