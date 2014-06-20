using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class WithTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
        public void Should_parse_a_with_binding()
        {
            var model = new { User = new { Name = "" }, Dept = new { Name = "" } };
            var template = Parse("Hello {{#with User}}{{Name}}{{/with}} from {{#with Dept}}{{Name}}{{/with}}", model.GetType());
            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.WriteExpression(Expression.SubModel(Expression.Property(model.GetType(), "User"), Expression.Property(model.User.GetType(), "Name")), true),
                SyntaxTree.WriteString(" from "),
                SyntaxTree.WriteExpression(Expression.SubModel(Expression.Property(model.GetType(), "Dept"), Expression.Property(model.Dept.GetType(), "Name")), true)
            );
        }

        [Test]
        public void Should_support_this_inside_with_block()
        {
            var model = new { User = "" };
            var template = Parse("Hello {{#with User}}{{this}}{{/with}}", model.GetType());
            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.WriteExpression(Expression.Property(model.GetType(), "User"), true)
            );
        }

        [Test]
        public void Should_support_parent_inside_with_block()
        {
            var model = new { User = "", Name = "" };
            var template = Parse("Hello {{#with User}}{{../Name}}{{this}}{{/with}}", model.GetType());
            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.WriteExpression(Expression.Property(model.GetType(), "Name"), true),
                SyntaxTree.WriteExpression(Expression.Property(model.GetType(), "User"), true)
            );
        }
    }
}