using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class ConditionalTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
        public void Should_parse_if_statement()
        {
            var template = Parse("Hello {{#if Conditional }} John{{/if}}", typeof(TestModel));
            AssertSyntaxTree(template, new SyntaxTreeNode[] {
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.Conditional(SyntaxTreeExpression.Property(typeof(TestModel), "Conditional"),
                    SyntaxTree.Block(SyntaxTree.WriteString(" John"))
                )
            });
        }

        [Test]
        public void Should_parse_else_statement()
        {
            var template = Parse("Hello {{#if Conditional }}John{{else}}Jim{{/if}}", typeof(TestModel));
            AssertSyntaxTree(template, new SyntaxTreeNode[] {
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.Conditional(SyntaxTreeExpression.Property(typeof(TestModel), "Conditional"),
                    SyntaxTree.Block(SyntaxTree.WriteString("John")),
                    SyntaxTree.Block(SyntaxTree.WriteString("Jim"))
                )
            });
        }

        [Test]
        public void Should_throw_if_property_not_on_model()
        {
            Assert.Throws<VeilParserException>(() =>
            {
                Parse("Hello {{#if NotFound}} There", typeof(TestModel));
            });
        }

        [TestCase("Hello {{#if Conditional}} There")]
        [TestCase("Hello {{#if Conditional}} There {{else}}")]
        [TestCase("Hello {{/if}} There")]
        [TestCase("Hello {{#if Conditional}} There{{/if}}{{/if}}")]
        [TestCase("Hello {{else}} Foo")]
        [TestCase("Hello {{#if Conditional}} There{{/if}}{{else}}")]
        public void Should_throw_if_block_not_open_and_closed_consistently(string template)
        {
            Assert.Throws<VeilParserException>(() =>
            {
                Parse(template, typeof(TestModel));
            });
        }

        private class TestModel
        {
            public bool Conditional { get; set; }
        }
    }
}