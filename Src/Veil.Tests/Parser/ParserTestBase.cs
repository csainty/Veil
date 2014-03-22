using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

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

        protected void AssertTemplate(TemplateRootNode template, params ISyntaxTreeNode[] nodes)
        {
            var resultNodes = template.TemplateNodes.ToArray();
            Assert.That(resultNodes, Has.Length.EqualTo(nodes.Length));
        }
    }
}