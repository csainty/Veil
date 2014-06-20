using NUnit.Framework;
using Veil.Parser;

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
                SyntaxTree.WriteString("Hello World")
            );
        }
    }
}