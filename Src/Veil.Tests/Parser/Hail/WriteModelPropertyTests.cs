using System.Linq;
using NUnit.Framework;

namespace Veil.Parser.Hail
{
    [TestFixture]
    internal class WriteModelPropertyTests : ParserTestBase<HailTemplateParser>
    {
        [TestCase("{{Name}}", new[] { "Name" })]
        [TestCase("{{ Name }}", new[] { "Name" })]
        [TestCase("Hello {{Name}}", new[] { "Name" })]
        [TestCase("Hello {{Name}}, {{ Greeting }}", new[] { "Name", "Greeting" })]
        public void Should_parse_model_property_names(string template, string[] expectedPropertyNames)
        {
            var model = new { Name = "", Greeting = "" };
            var syntaxTree = Parse(template, model.GetType());
            var propertyNames = syntaxTree.TemplateNodes.OfType<WriteModelPropertyNode>().Select(x => x.ModelProperty.Name);
            Assert.That(propertyNames, Is.EquivalentTo(expectedPropertyNames));
        }

        [TestCase]
        public void Should_throw_if_identifier_is_incomplete()
        {
            var model = new { Name = "" };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse("Hello {{ Name} !", model.GetType());
            });
        }

        [TestCase]
        public void Should_throw_if_property_not_part_OF_model()
        {
            var model = new { Name = "" };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse("Hello {{ name }} !", model.GetType());
            });
        }

        [TestCase]
        public void Should_handle_incomplete_identifier_marker()
        {
            var syntaxTree = Parse("Hello {Name}", typeof(object));
            var result = syntaxTree.TemplateNodes.OfType<WriteLiteralNode>().Single();
            Assert.That(result.LiteralContent, Is.EqualTo("Hello {Name}"));
        }
    }
}