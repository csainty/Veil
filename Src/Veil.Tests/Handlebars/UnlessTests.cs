using NUnit.Framework;
using E = Veil.Parser.SyntaxTreeNode.ExpressionNode;
using S = Veil.Parser.SyntaxTreeNode;

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
                S.WriteString("Hello "),
                S.Conditional(
                    E.Property(model.GetType(), "Test"),
                    S.Block(),
                    S.Block(
                        S.WriteString("World")
                    )
                )
            );
        }
    }
}