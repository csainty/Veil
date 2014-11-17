using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class WriteExpressionTests : ParserTestBase<HandlebarsParser>
    {
        [TestCaseSource("PropertyNameTestSource")]
        public void Should_parse_model_property_names(string template, SyntaxTreeNode[] expectedTemplate)
        {
            var syntaxTree = Parse(template, typeof(TestModel));
            AssertSyntaxTree(syntaxTree, expectedTemplate);
        }

        public object[] PropertyNameTestSource()
        {
            return new object[] {
                new object[] {"{{Name}}", new SyntaxTreeNode[] { SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), true) } },
                new object[] {"{{ Name }}", new SyntaxTreeNode[] { SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), true) } },
                new object[] {"Hello {{Name}}", new SyntaxTreeNode[] { SyntaxTree.WriteString("Hello "), SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), true) } },
                new object[] {"Hello {{Name}}, {{ Greeting }}", new SyntaxTreeNode[] { SyntaxTree.WriteString("Hello "), SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), true), SyntaxTree.WriteString(", "), SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Greeting"), true) } },
                new object[] {"{{{ Name }}}", new SyntaxTreeNode[] { SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), false) }}
            };
        }

        [Test]
        public void Should_throw_if_property_not_part_OF_model()
        {
            var model = new { Name = "" };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse("Hello {{ foo }} !", model.GetType());
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
        [TestCase("{{{{ Name }}}}")]
        [TestCase("Hello {{{{ Name }}}}")]
        [TestCase("Hello {{{{ Name }}}} World")]
        public void Should_handle_incomplete_identifier_marker(string testString)
        {
            var syntaxTree = Parse(testString, typeof(object));
            AssertSyntaxTree(syntaxTree, SyntaxTree.WriteString(testString));
        }

        private class TestModel
        {
            public string Name { get; set; }

            public string Greeting { get; set; }
        }
    }
}