using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class WhitespaceControlTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
        public void Should_trim_whitespace_from_previous_literal()
        {
            var template = Parse("Hello \r\n{{~this}}", typeof(string));
            AssertSyntaxTree(
                template,
                SyntaxTreeNode.WriteString("Hello"),
                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Self(typeof(string)), true)
            );
        }

        [Test]
        public void Should_trim_whitespace_from_next_literal()
        {
            var template = Parse("Hello {{this~}}\r\n!", typeof(string));
            AssertSyntaxTree(
                template,
                SyntaxTreeNode.WriteString("Hello "),
                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Self(typeof(string)), true),
                SyntaxTreeNode.WriteString("!")
            );
        }
    }
}