using System;
using System.Collections.Generic;

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

        public static WriteLiteralNode StringLiteral(string content)
        {
            return new WriteLiteralNode
            {
                LiteralContent = content,
                LiteralType = typeof(string)
            };
        }

        public static WriteModelExpressionNode Expression(Type type, string expression)
        {
            return new WriteModelExpressionNode { Expression = ExpressionParser.Parse(type, expression) };
        }

        public static EachNode Each(ModelPropertyExpressionNode collectionExpression, BlockNode body)
        {
            return new EachNode
            {
                Collection = collectionExpression,
                Body = body
            };
        }

        public static ConditionalOnModelExpressionNode Conditional(Type type, string propertyExpression, BlockNode trueBlock, BlockNode falseBlock = null)
        {
            return new ConditionalOnModelExpressionNode
            {
                Expression = ExpressionParser.Parse(type, propertyExpression),
                TrueBlock = trueBlock,
                FalseBlock = falseBlock
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

        public class WriteModelExpressionNode : SyntaxTreeNode
        {
            public ModelExpressionNode Expression { get; set; }
        }

        public class ConditionalOnModelExpressionNode : SyntaxTreeNode
        {
            public ModelExpressionNode Expression { get; set; }

            public BlockNode TrueBlock { get; set; }

            public BlockNode FalseBlock { get; set; }
        }

        public class EachNode : SyntaxTreeNode
        {
            public ModelExpressionNode Collection { get; set; }

            public BlockNode Body { get; set; }
        }
    }
}