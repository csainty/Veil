using System;
using System.Collections.Generic;

namespace Veil
{
    public interface ISyntaxTreeNode { }

    public interface IBlockNode : ISyntaxTreeNode
    {
        IEnumerable<ISyntaxTreeNode> Nodes { get; }
    }

    public class TemplateRootNode : BlockNode
    {
    }

    public class BlockNode : IBlockNode
    {
        private List<ISyntaxTreeNode> nodes;

        public BlockNode()
        {
            this.nodes = new List<ISyntaxTreeNode>();
        }

        public void Add(ISyntaxTreeNode node)
        {
            this.nodes.Add(node);
        }

        public void AddRange(IEnumerable<ISyntaxTreeNode> nodes)
        {
            this.nodes.AddRange(nodes);
        }

        public IEnumerable<ISyntaxTreeNode> Nodes { get { return this.nodes; } }

        public static BlockNode Create(IEnumerable<ISyntaxTreeNode> nodes)
        {
            var block = new BlockNode();
            block.AddRange(nodes);
            return block;
        }
    }

    public class WriteLiteralNode : ISyntaxTreeNode
    {
        public Type LiteralType { get; set; }

        public object LiteralContent { get; set; }

        public static WriteLiteralNode String(string value)
        {
            return new WriteLiteralNode { LiteralContent = value, LiteralType = typeof(string) };
        }
    }

    public class WriteModelExpressionNode : ISyntaxTreeNode
    {
        public IModelExpressionNode Expression { get; set; }

        public static WriteModelExpressionNode Create(Type type, string propertyExpression)
        {
            return new WriteModelExpressionNode { Expression = ExpressionParser.Parse(type, propertyExpression) };
        }
    }

    public class ConditionalOnModelExpressionNode : ISyntaxTreeNode
    {
        public IModelExpressionNode Expression { get; set; }

        public IBlockNode TrueBlock { get; set; }

        public IBlockNode FalseBlock { get; set; }

        public static ConditionalOnModelExpressionNode Create(Type type, string propertyExpression, IBlockNode trueBlock, IBlockNode falseBlock = null)
        {
            return new ConditionalOnModelExpressionNode
            {
                Expression = ExpressionParser.Parse(type, propertyExpression),
                TrueBlock = trueBlock,
                FalseBlock = falseBlock
            };
        }

        public static ConditionalOnModelExpressionNode Create(Type type, string propertyExpression, IEnumerable<ISyntaxTreeNode> trueBlock, IEnumerable<ISyntaxTreeNode> falseBlock = null)
        {
            return ConditionalOnModelExpressionNode.Create(type, propertyExpression, BlockNode.Create(trueBlock), falseBlock == null ? null : BlockNode.Create(falseBlock));
        }
    }

    public class EachNode : ISyntaxTreeNode
    {
        public IModelExpressionNode Collection { get; set; }

        public BlockNode Body { get; set; }

        internal static EachNode Create(ModelPropertyExpressionNode collection, BlockNode body)
        {
            return new EachNode
            {
                Collection = collection,
                Body = body
            };
        }
    }
}