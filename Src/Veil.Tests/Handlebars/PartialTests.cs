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
                SyntaxTreeNode.WriteString("Hello "),
                SyntaxTreeNode.Include("person", SyntaxTreeNode.ExpressionNode.Self(model.GetType()))
            );
        }
    }
}