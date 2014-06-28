using System.Collections.Generic;
using Veil.Parser.Nodes;

namespace Veil.Parser
{
    /// <summary>
    /// Factory methods for creating syntax tree nodes
    /// </summary>
    public static class SyntaxTree
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
                LiteralContent = content
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
        /// Iterate a collection and execute the body block scoped to each item in the collection.
        /// Optionally execute an empty block when there are no items to iterate
        /// </summary>
        /// <param name="collectionExpression">expression to load the collection</param>
        /// <param name="body">Block to execute in the scope of each item</param>
        /// <param name="emptyBody">Block to execute when there are no items in the collection</param>
        public static IterateNode Iterate(ExpressionNode collectionExpression, BlockNode body, BlockNode emptyBody = null)
        {
            return new IterateNode
            {
                Collection = collectionExpression,
                Body = body,
                EmptyBody = emptyBody ?? SyntaxTree.Block()
            };
        }

        /// <summary>
        /// Evaluates an expression and chooses between two blocks based on the truthy-ness of the result
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
        /// Execute another template in the scope of the provided model
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

        /// <summary>
        /// Scopes a node to a new model
        /// </summary>
        /// <param name="modelToScopeTo">An expression that evaluates to the model to scope to</param>
        /// <param name="node">The node to execute in the new scope</param>
        public static ScopedNode ScopeNode(ExpressionNode modelToScopeTo, SyntaxTreeNode node)
        {
            return new ScopedNode
            {
                ModelToScope = modelToScopeTo,
                Node = node
            };
        }
    }
}