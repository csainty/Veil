using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class UnlessTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
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

        [TestCase("Hello {{#unless Conditional}} There")]
        [TestCase("Hello {{/unless}} There")]
        [TestCase("Hello {{#unless Conditional}} There{{/unless}}{{/unless}}")]
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