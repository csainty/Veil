using System;
using System.Collections.Generic;
using System.Reflection;

namespace Veil
{
    internal interface ISyntaxTreeNode { }

    internal class TemplateRootNode : BlockNode
    {
    }

    internal class BlockNode : ISyntaxTreeNode
    {
        public IEnumerable<ISyntaxTreeNode> TemplateNodes { get; set; }
    }

    internal class WriteLiteralNode : ISyntaxTreeNode
    {
        public Type LiteralType { get; set; }

        public object LiteralContent { get; set; }
    }

    internal class WriteModelPropertyNode : ISyntaxTreeNode
    {
        public PropertyInfo ModelProperty { get; set; }
    }

    internal class ConditionalOnModelPropertyNode : ISyntaxTreeNode
    {
        public PropertyInfo ModelProperty { get; set; }

        public BlockNode TrueBlock { get; set; }

        public BlockNode FalseBlock { get; set; }
    }
}