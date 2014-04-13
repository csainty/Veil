using NUnit.Framework;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class WriteLiteralTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
        public void Should_parse_template_without_expressions_as_literal()
        {
            var result = Parse("Hello World");
            AssertSyntaxTree(
                result,
                new WriteLiteralNode { LiteralContent = "Hello World", LiteralType = typeof(string) }
            );
        }
    }
}