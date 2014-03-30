using NUnit.Framework;

namespace Veil.Parser.Hail
{
    [TestFixture]
    internal class WriteLiteralTests : ParserTestBase<HailTemplateParser>
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