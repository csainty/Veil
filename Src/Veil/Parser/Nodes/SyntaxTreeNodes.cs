using System;
using System.Collections.Generic;

namespace Veil.Parser.Nodes
{
    public class BlockNode : SyntaxTreeNode
    {
        private List<SyntaxTreeNode> nodes;

        public BlockNode()
        {
            this.nodes = new List<SyntaxTreeNode>();
        }

        public void Add(SyntaxTreeNode node)
        {
            this.nodes.Add(node);
        }

        public void AddRange(IEnumerable<SyntaxTreeNode> nodes)
        {
            this.nodes.AddRange(nodes);
        }

        public IEnumerable<SyntaxTreeNode> Nodes { get { return this.nodes; } }
    }

    public class WriteLiteralNode : SyntaxTreeNode
    {
        public string LiteralContent { get; set; }
    }

    public class WriteExpressionNode : SyntaxTreeNode
    {
        public ExpressionNode Expression { get; set; }

        public bool HtmlEncode { get; set; }
    }

    public class ConditionalNode : SyntaxTreeNode
    {
        private ExpressionNode expression;

        public ExpressionNode Expression { get { return this.expression; } set { this.expression = value; this.Validate(); } }

        public BlockNode TrueBlock { get; set; }

        public BlockNode FalseBlock { get; set; }

        private void Validate()
        {
            if (expression.ResultType.IsValueType && expression.ResultType != typeof(bool)) throw new VeilParserException("Attempted to use a ValueType other than bool as the expression in a conditional.");
        }
    }

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

    public class IncludeTemplateNode : SyntaxTreeNode
    {
        public ExpressionNode ModelExpression { get; set; }

        public string TemplateName { get; set; }
    }

    public class ExtendTemplateNode : SyntaxTreeNode
    {
        public string TemplateName { get; set; }

        public IDictionary<string, SyntaxTreeNode> Overrides { get; set; }
    }

    public class OverridePointNode : SyntaxTreeNode
    {
        public string OverrideName { get; set; }

        public bool IsRequired { get; set; }

        public SyntaxTreeNode DefaultContent { get; set; }
    }

    public class FlushNode : SyntaxTreeNode
    {
    }
}