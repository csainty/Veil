using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class WriteLiteralTests : ParserTestBase<HandlebarsParser>
    {
        [Fact]
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