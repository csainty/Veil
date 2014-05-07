using System;
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

        /// <summary>
        /// Defines a template that extends another template e.g. MasterPages
        /// Extend nodes must be the root of a syntax tree
        /// </summary>
        /// <param name="templateName">The name of the template to extend. It will be loaded from the <see cref="IVeilContext"/></param>
        /// <param name="overrides">A set of overrides for the <see cref="OverridePointNode"/> defined in the template being extended.</param>
        public static ExtendTemplateNode Extend(string templateName, IDictionary<string, SyntaxTreeNode> overrides = null)
        {
            return new ExtendTemplateNode
            {
                TemplateName = templateName,
                Overrides = overrides ?? new Dictionary<string, SyntaxTreeNode>()
            };
        }

        /// <summary>
        /// Defines a point in a template that can be overridden when the template is extended.
        /// </summary>
        /// <param name="overrideName">The name of the override which must match that specified in the overriding template</param>
        /// <param name="isOptional">Indicates whether an exception should be thrown if the override is missing</param>
        public static OverridePointNode Override(string overrideName, bool isOptional = false)
        {
            return new OverridePointNode
            {
                OverrideName = overrideName,
                IsRequired = !isOptional
            };
        }

        /// <summary>
        /// Defines an optional point in a template that can be overridden when the template is extended.
        /// If the point is not overridden then the specified content is used by default
        /// </summary>
        /// <param name="overrideName">The name of the override which must match that specified in the overriding template</param>
        /// <param name="defaultContent">The content to use when the point is not overridden</param>
        public static OverridePointNode Override(string overrideName, SyntaxTreeNode defaultContent)
        {
            return new OverridePointNode
            {
                OverrideName = overrideName,
                IsRequired = false,
                DefaultContent = defaultContent
            };
        }

        /// <summary>
        /// Flushes the TextWriter.
        /// Used to optimize responses in web applications.
        /// </summary>
        public static FlushNode Flush()
        {
            return new FlushNode();
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
}