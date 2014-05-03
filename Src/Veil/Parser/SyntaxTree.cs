﻿using System;
using System.Collections.Generic;

namespace Veil.Parser
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
        public static WriteLiteralNode WriteString(string content)
        {
            return new WriteLiteralNode
            {
                LiteralContent = content,
                LiteralType = typeof(string)
            };
        }

        /// <summary>
        /// Evaluate an expression and write the value to the TextWriter
        /// </summary>
        /// <param name="expression">The expression to be written</param>
        /// <param name="htmlEncode">Indicates whether the content should be html encoded before being written</param>
        public static WriteExpressionNode WriteExpression(ExpressionNode expression, bool htmlEncode = false)
        {
            return new WriteExpressionNode
            {
                Expression = expression,
                HtmlEncode = htmlEncode
            };
        }

        /// <summary>
        /// Iterate a collection and execute the body block scoped to each item in the collection
        /// </summary>
        /// <param name="collectionExpression">expression to load the collection</param>
        /// <param name="body">Block to execute in the scope of each item</param>
        public static IterateNode Iterate(ExpressionNode collectionExpression, BlockNode body)
        {
            return new IterateNode
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
        public static ConditionalNode Conditional(ExpressionNode expression, BlockNode trueBlock, BlockNode falseBlock = null)
        {
            return new ConditionalNode
            {
                Expression = expression,
                TrueBlock = trueBlock,
                FalseBlock = falseBlock
            };
        }

        /// <summary>
        /// Execute another template with the provided model and output the result
        /// </summary>
        /// <param name="templateName">The name of the template to execute. It will be loaded from the <see cref="IVeilContext"/></param>
        /// <param name="modelExpression">An expression for the model to be used as the root scope when executing the template</param>
        public static IncludeTemplateNode Include(string templateName, ExpressionNode modelExpression)
        {
            return new IncludeTemplateNode
            {
                ModelExpression = modelExpression,
                TemplateName = templateName
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

        public class WriteExpressionNode : SyntaxTreeNode
        {
            public ExpressionNode Expression { get; set; }

            public bool HtmlEncode { get; set; }
        }

        public class ConditionalNode : SyntaxTreeNode
        {
            public ExpressionNode Expression { get; set; }

            public BlockNode TrueBlock { get; set; }

            public BlockNode FalseBlock { get; set; }
        }

        public class IterateNode : SyntaxTreeNode
        {
            private ExpressionNode collection;

            public ExpressionNode Collection { get { return this.collection; } set { this.collection = value; this.ValidateCollection(); } }

            private void ValidateCollection()
            {
                if (!this.collection.ResultType.HasEnumerableInterface())
                {
                    throw new VeilParserException("Expression used as iteration collection is not IEnumerable<>");
                }
            }

            public BlockNode Body { get; set; }

            public Type ItemType { get { return Collection.ResultType.GetEnumerableInterface().GetGenericArguments()[0]; } }
        }

        public class IncludeTemplateNode : SyntaxTreeNode
        {
            public ExpressionNode ModelExpression { get; set; }

            public string TemplateName { get; set; }
        }
    }
}