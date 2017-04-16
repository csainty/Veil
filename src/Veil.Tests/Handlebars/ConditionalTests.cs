using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class ConditionalTests : ParserTestBase<HandlebarsParser>
    {
        [Fact]
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

        [Fact]
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

        [Fact]
        public void Should_throw_if_property_not_on_model()
        {
            Assert.Throws<VeilParserException>(() =>
            {
                Parse("Hello {{#if NotFound}} There", typeof(TestModel));
            });
        }

        [Theory]
        [InlineData("Hello {{#if Conditional}} There")]
        [InlineData("Hello {{#if Conditional}} There {{else}}")]
        [InlineData("Hello {{/if}} There")]
        [InlineData("Hello {{#if Conditional}} There{{/if}}{{/if}}")]
        [InlineData("Hello {{else}} Foo")]
        [InlineData("Hello {{#if Conditional}} There{{/if}}{{else}}")]
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