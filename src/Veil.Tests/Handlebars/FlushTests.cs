using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class FlushTests : ParserTestBase<HandlebarsParser>
    {
        [Fact]
        public void Should_parse_flush()
        {
            var syntaxTree = Parse("{{#flush}}", typeof(object));
            AssertSyntaxTree(syntaxTree, SyntaxTree.Flush());
        }
    }
}