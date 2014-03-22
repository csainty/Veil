using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veil
{
    internal interface ISyntaxTreeNode { }

    internal class TemplateRootNode : ISyntaxTreeNode
    {
        public IEnumerable<ISyntaxTreeNode> TemplateNodes { get; set; }
    }

    internal class WriteLiteralNode : ISyntaxTreeNode
    {
        public Type LiteralType { get; set; }

        public object LiteralContent { get; set; }
    }
}
