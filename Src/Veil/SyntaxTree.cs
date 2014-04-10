using System;
using System.Collections.Generic;
using System.Reflection;

namespace Veil
{
    internal interface ISyntaxTreeNode { }

    internal interface IModelPropertyNode : ISyntaxTreeNode
    {
        Type Type { get; }
    }

    internal interface IBlockNode : ISyntaxTreeNode
    {
        IEnumerable<ISyntaxTreeNode> Nodes { get; }
    }

    internal class TemplateRootNode : BlockNode
    {
    }

    internal class BlockNode : IBlockNode
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

    internal class WriteLiteralNode : ISyntaxTreeNode
    {
        public Type LiteralType { get; set; }

        public object LiteralContent { get; set; }

        public static WriteLiteralNode String(string value)
        {
            return new WriteLiteralNode { LiteralContent = value, LiteralType = typeof(string) };
        }
    }

    internal class ModelProperty : IModelPropertyNode
    {
        public PropertyInfo Property { get; set; }

        public Type Type
        {
            get { return this.Property.PropertyType; }
        }

        public static ModelProperty Create(Type type, string propertyName)
        {
            return new ModelProperty { Property = type.GetProperty(propertyName) };
        }
    }

    internal class WriteModelPropertyNode : ISyntaxTreeNode
    {
        public IModelPropertyNode Property { get; set; }

        public static WriteModelPropertyNode Create(Type type, string propertyExpression)
        {
            return new WriteModelPropertyNode { Property = ExpressionParser.Parse(type, propertyExpression) };
        }
    }

    internal class ConditionalOnModelPropertyNode : ISyntaxTreeNode
    {
        public IModelPropertyNode Property { get; set; }

        public IBlockNode TrueBlock { get; set; }

        public IBlockNode FalseBlock { get; set; }

        public static ConditionalOnModelPropertyNode Create(Type type, string propertyExpression, IBlockNode trueBlock, IBlockNode falseBlock = null)
        {
            return new ConditionalOnModelPropertyNode
            {
                Property = ExpressionParser.Parse(type, propertyExpression),
                TrueBlock = trueBlock,
                FalseBlock = falseBlock
            };
        }

        public static ConditionalOnModelPropertyNode Create(Type type, string propertyExpression, IEnumerable<ISyntaxTreeNode> trueBlock, IEnumerable<ISyntaxTreeNode> falseBlock = null)
        {
            return ConditionalOnModelPropertyNode.Create(type, propertyExpression, BlockNode.Create(trueBlock), falseBlock == null ? null : BlockNode.Create(falseBlock));
        }
    }
}