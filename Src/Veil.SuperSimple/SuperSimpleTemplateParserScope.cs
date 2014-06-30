using System;
using Veil.Parser.Nodes;

namespace Veil.SuperSimple
{
    internal class SuperSimpleTemplateParserScope
    {
        public BlockNode Block { get; set; }

        public Type ModelType { get; set; }
    }
}