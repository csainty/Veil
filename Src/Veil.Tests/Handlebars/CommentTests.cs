using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class CommentTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
        public void Should_ignore_comments()
        {
            var template = Parse("Hello {{! this again? }}World{{! at least it is done now}}", typeof(object));
            AssertSyntaxTree(
                template,
                SyntaxTreeNode.WriteString("Hello "),
                SyntaxTreeNode.WriteString("World")
            );
        }
    }
}