using System;
using System.Collections.Generic;
using System.Linq;

namespace Veil
{
    public abstract class SyntaxTreeNode
    {
        public static BlockNode Block(params SyntaxTreeNode[] nodes)
        {
            var block = new BlockNode();
            block.AddRange(nodes);
            return block;
        }

        public static WriteLiteralNode StringLiteral(string value)
        {
            return new WriteLiteralNode
            {
                LiteralContent = value,
                LiteralType = typeof(string)
            };
        }

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
            public Type LiteralType { get; set; }

            public object LiteralContent { get; set; }
        }
    }

    public class WriteModelExpressionNode : SyntaxTreeNode
    {
        public ModelExpressionNode Expression { get; set; }

        public static WriteModelExpressionNode Create(Type type, string propertyExpression)
        {
            return new WriteModelExpressionNode { Expression = ExpressionParser.Parse(type, propertyExpression) };
        }
    }

    public class ConditionalOnModelExpressionNode : SyntaxTreeNode
    {
        public ModelExpressionNode Expression { get; set; }

        public BlockNode TrueBlock { get; set; }

        public BlockNode FalseBlock { get; set; }

        public static ConditionalOnModelExpressionNode Create(Type type, string propertyExpression, BlockNode trueBlock, BlockNode falseBlock = null)
        {
            return new ConditionalOnModelExpressionNode
            {
                Expression = ExpressionParser.Parse(type, propertyExpression),
                TrueBlock = trueBlock,
                FalseBlock = falseBlock
            };
        }

        public static ConditionalOnModelExpressionNode Create(Type type, string propertyExpression, IEnumerable<SyntaxTreeNode> trueBlock, IEnumerable<SyntaxTreeNode> falseBlock = null)
        {
            return ConditionalOnModelExpressionNode.Create(type, propertyExpression, SyntaxTreeNode.Block(trueBlock.ToArray()), falseBlock == null ? null : SyntaxTreeNode.Block(falseBlock.ToArray()));
        }
    }

    public class EachNode : SyntaxTreeNode
    {
        public ModelExpressionNode Collection { get; set; }

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