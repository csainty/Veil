using NUnit.Framework;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class WriteModelPropertyTests : ParserTestBase<HandlebarsParser>
    {
        [TestCaseSource("PropertyNameTestSource")]
        public void Should_parse_model_property_names(string template, ISyntaxTreeNode[] expectedTemplate)
        {
            var syntaxTree = Parse(template, typeof(TestModel));
            AssertSyntaxTree(syntaxTree, expectedTemplate);
        }

        public object[] PropertyNameTestSource()
        {
            return new object[] {
                new object[] {"{{Name}}", new ISyntaxTreeNode[] { WriteModelExpressionNode.Create(typeof(TestModel), "Name") } },
                new object[] {"{{ Name }}", new ISyntaxTreeNode[] { WriteModelExpressionNode.Create(typeof(TestModel), "Name") } },
                new object[] {"Hello {{Name}}", new ISyntaxTreeNode[] { WriteLiteralNode.String("Hello "), WriteModelExpressionNode.Create(typeof(TestModel), "Name") } },
                new object[] {"Hello {{Name}}, {{ Greeting }}", new ISyntaxTreeNode[] { WriteLiteralNode.String("Hello "), WriteModelExpressionNode.Create(typeof(TestModel), "Name"), WriteLiteralNode.String(", "), WriteModelExpressionNode.Create(typeof(TestModel), "Greeting") } }
            };
        }

        [Test]
        public void Should_throw_if_property_not_part_OF_model()
        {
            var model = new { Name = "" };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse("Hello {{ name }} !", model.GetType());
            });
        }

        [TestCase("{Name}")]
        [TestCase("Hello {Name}")]
        [TestCase("Hello { this string { contains { opening identifiers")]
        [TestCase("{{ name }")]
        [TestCase("Hello {{ name }")]
        [TestCase("Hello {{ name } World")]
        [TestCase("{ name }}")]
        [TestCase("Hello { name }}")]
        [TestCase("Hello { name }} World")]
        [TestCase("{{{ Name }}")]
        [TestCase("{{{ Name }}}")]
        [TestCase("Hello {{{ Name }}}")]
        [TestCase("Hello {{{ Name }}} World")]
        public void Should_handle_incomplete_identifier_marker(string testString)
        {
            var syntaxTree = Parse(testString, typeof(object));
            AssertSyntaxTree(syntaxTree, WriteLiteralNode.String(testString));
        }

        private class TestModel
        {
            public string Name { get; set; }

            public string Greeting { get; set; }
        }
    }
}