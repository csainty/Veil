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
                    Expression.Property(model.GetType(), "Test"),
                    SyntaxTree.Block(),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("World")
                    )
                )
            );
        }
    }
}