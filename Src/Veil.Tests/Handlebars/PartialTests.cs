using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class PartialTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
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