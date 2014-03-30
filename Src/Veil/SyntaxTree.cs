using System;
using System.Collections.Generic;
using System.Reflection;

namespace Veil
{
    internal interface ISyntaxTreeNode { }

    internal interface ISyntaxTreeBlockNode : ISyntaxTreeNode
    {
        IEnumerable<ISyntaxTreeNode> Nodes { get; }
    }

    internal class TemplateRootNode : BlockNode
    {
    }

    internal class BlockNode : ISyntaxTreeBlockNode
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

    internal class WriteModelPropertyNode : ISyntaxTreeNode
    {
        public PropertyInfo ModelProperty { get; set; }

        public static WriteModelPropertyNode Create(Type type, string propertyName)
        {
            return new WriteModelPropertyNode { ModelProperty = type.GetProperty(propertyName) };
        }
    }

    internal class ConditionalOnModelPropertyNode : ISyntaxTreeNode
    {
        public PropertyInfo ModelProperty { get; set; }

        public ISyntaxTreeBlockNode TrueBlock { get; set; }

        public ISyntaxTreeBlockNode FalseBlock { get; set; }

        public static ConditionalOnModelPropertyNode Create(Type type, string propertyName, IEnumerable<ISyntaxTreeNode> trueBlock, IEnumerable<ISyntaxTreeNode> falseBlock = null)
        {
            return new ConditionalOnModelPropertyNode
            {
                ModelProperty = type.GetProperty(propertyName),
                TrueBlock = BlockNode.Create(trueBlock),
                FalseBlock = falseBlock != null ? BlockNode.Create(falseBlock) : null
            };
        }
    }
}