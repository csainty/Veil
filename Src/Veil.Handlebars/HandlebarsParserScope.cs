using System;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    internal class HandlebarsParserScope
    {
        public BlockNode Block { get; set; }

        public Type ModelInScope { get; set; }
    }
}