using System;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    internal class HandlebarsParserBlock
    {
        public BlockNode Block { get; set; }

        public Type ModelInScope { get; set; }
    }
}