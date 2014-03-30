using System;
using System.IO;
using System.Linq;
using DeepEqual.Syntax;

namespace Veil.Parser
{
    internal class ParserTestBase<T> where T : ITemplateParser, new()
    {
        protected T parser;

        public ParserTestBase()
        {
            this.parser = new T();
        }

        protected TemplateRootNode Parse(string template, Type modelType = null)
        {
            using (var reader = new StringReader(template))
            {
                return this.parser.Parse(reader, modelType ?? typeof(object));
            }
        }

        protected void AssertSyntaxTree(TemplateRootNode template, params ISyntaxTreeNode[] expectedNodes)
        {
            var resultNodes = template.Nodes.ToArray();
            var comparisonTemplate = new TemplateRootNode();
            comparisonTemplate.AddRange(expectedNodes);
            template.ShouldDeepEqual(comparisonTemplate);
        }
    }
}