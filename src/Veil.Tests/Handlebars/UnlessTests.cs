using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class UnlessTests : ParserTestBase<HandlebarsParser>
    {
        [Fact]
        public void Should_parse_unless_block()
        {
            var model = new { Test = true };
            var template = Parse("Hello {{#unless Test}}World{{/unless}}", model.GetType());
            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.Property(model.GetType(), "Test"),
                    SyntaxTree.Block(),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("World")
                    )
                )
            );
        }

        [Theory]
        [InlineData("Hello {{#unless Conditional}} There")]
        [InlineData("Hello {{/unless}} There")]
        [InlineData("Hello {{#unless Conditional}} There{{/unless}}{{/unless}}")]
        public void Should_throw_if_block_not_open_and_closed_consistently(string template)
        {
            var model = new { Conditional = false };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse(template, model.GetType());
            });
        }
    }
}