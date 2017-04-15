using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class PartialTests : ParserTestBase<HandlebarsParser>
    {
        [Fact]
        public void Should_parse_partial_syntax()
        {
            var model = new { };
            var template = Parse("Hello {{> person }}", model.GetType());

            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.Include("person", SyntaxTreeExpression.Self(model.GetType()))
            );
        }
    }
}