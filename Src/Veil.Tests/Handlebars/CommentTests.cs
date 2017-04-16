using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class CommentTests : ParserTestBase<HandlebarsParser>
    {
        [Fact]
        public void Should_ignore_comments()
        {
            var template = Parse("Hello {{! this again? }}World{{! at least it is done now}}", typeof(object));
            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.WriteString("World")
            );
        }
    }
}