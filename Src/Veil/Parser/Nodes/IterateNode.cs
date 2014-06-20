using System;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node which iterates a collection and executes the body once in for each item.
    /// Optionally a alternative block can be rendered if there are no items
    /// </summary>
    public class IterateNode : SyntaxTreeNode
    {
        private ExpressionNode collection;

        public ExpressionNode Collection { get { return this.collection; } set { this.collection = value; this.ValidateCollection(); } }

        private void ValidateCollection()
        {
            if (this.collection.ResultType == typeof(object)) return;

            if (!this.collection.ResultType.HasEnumerableInterface())
            {
                throw new VeilParserException("Expression used as iteration collection is not IEnumerable<>");
            }
        }

        public BlockNode Body { get; set; }

        public BlockNode EmptyBody { get; set; }

        public Type ItemType
        {
            get
            {
                if (Collection.ResultType == typeof(object)) return Collection.ResultType;
                return Collection.ResultType.GetEnumerableInterface().GetGenericArguments()[0];
            }
        }
    }
}