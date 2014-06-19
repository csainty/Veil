using System;
using System.IO;
using DeepEqual.Syntax;
using Veil.Parser;

namespace Veil
{
    internal class ParserTestBase<T> where T : ITemplateParser, new()
    {
        protected T parser;

        public ParserTestBase()
        {
            this.parser = new T();
        }

        protected SyntaxTreeNode Parse(string template, Type modelType = null)
        {
            using (var reader = new StringReader(template))
            {
                return this.parser.Parse(reader, modelType ?? typeof(object));
            }
        }

        protected void AssertSyntaxTree(SyntaxTreeNode template, params SyntaxTreeNode[] expectedNodes)
        {
            var comparisonTemplate = SyntaxTree.Block(expectedNodes);
            template.ShouldDeepEqual(comparisonTemplate);
        }
    }
}