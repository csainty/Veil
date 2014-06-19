using NUnit.Framework;
using E = Veil.Parser.Expression;
using S = Veil.Parser.SyntaxTree;

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
                S.WriteString("Hello "),
                S.WriteExpression(E.SubModel(E.Property(model.GetType(), "User"), E.Property(model.User.GetType(), "Name")), true),
                S.WriteString(" from "),
                S.WriteExpression(E.SubModel(E.Property(model.GetType(), "Dept"), E.Property(model.Dept.GetType(), "Name")), true)
            );
        }

        [Test]
        public void Should_support_this_inside_with_block()
        {
            var model = new { User = "" };
            var template = Parse("Hello {{#with User}}{{this}}{{/with}}", model.GetType());
            AssertSyntaxTree(
                template,
                S.WriteString("Hello "),
                S.WriteExpression(E.Property(model.GetType(), "User"), true)
            );
        }

        [Test]
        public void Should_support_parent_inside_with_block()
        {
            var model = new { User = "", Name = "" };
            var template = Parse("Hello {{#with User}}{{../Name}}{{this}}{{/with}}", model.GetType());
            AssertSyntaxTree(
                template,
                S.WriteString("Hello "),
                S.WriteExpression(E.Property(model.GetType(), "Name"), true),
                S.WriteExpression(E.Property(model.GetType(), "User"), true)
            );
        }
    }
}