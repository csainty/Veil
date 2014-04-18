using System;
using System.Collections.Generic;

namespace Veil
{
    public abstract partial class SyntaxTreeNode
    {
        /// <summary>
        /// Create a sequential block of nodes
        /// </summary>
        public static BlockNode Block(params SyntaxTreeNode[] nodes)
        {
            var block = new BlockNode();
            block.AddRange(nodes);
            return block;
        }

        /// <summary>
        /// Write a string literal to the TextWriter
        /// </summary>
        /// <param name="content">The string to be written</param>
        public static WriteLiteralNode StringLiteral(string content)
        {
            return new WriteLiteralNode
            {
                LiteralContent = content,
                LiteralType = typeof(string)
            };
        }

        /// <summary>
        /// Evaluate an expression against the currently scoped model and write the value to the TextWriter
        /// </summary>
        public static WriteModelExpressionNode Expression(ExpressionNode expression)
        {
            return new WriteModelExpressionNode
            {
                Expression = expression
            };
        }

        /// <summary>
        /// Iterate a collection and execute the body block scoped to each item in the collection
        /// </summary>
        /// <param name="collectionExpression">expression to load the collection</param>
        /// <param name="body">Block to execute in the scope of each item</param>
        public static EachNode Each(ExpressionNode collectionExpression, BlockNode body)
        {
            return new EachNode
            {
                Collection = collectionExpression,
                Body = body
            };
        }

        /// <summary>
        /// Choose a Block to execute based on a condition
        /// </summary>
        /// <param name="expression">The expression to evaluate</param>
        /// <param name="trueBlock">The block to execute when the expression is true</param>
        /// <param name="falseBlock">The block to evaluate when the expression is false</param>
        /// <returns></returns>
        public static ConditionalOnModelExpressionNode Conditional(ExpressionNode expression, BlockNode trueBlock, BlockNode falseBlock = null)
        {
            return new ConditionalOnModelExpressionNode
            {
                Expression = expression,
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
            public ExpressionNode Expression { get; set; }
        }

        public class ConditionalOnModelExpressionNode : SyntaxTreeNode
        {
            public ExpressionNode Expression { get; set; }

            public BlockNode TrueBlock { get; set; }

            public BlockNode FalseBlock { get; set; }
        }

        public class EachNode : SyntaxTreeNode
        {
            public ExpressionNode Collection { get; set; }

            public BlockNode Body { get; set; }

            public Type ItemType { get { return Collection.ResultType.GetEnumerableInterface().GetGenericArguments()[0]; } }
        }
    }
}