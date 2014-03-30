using NUnit.Framework;

namespace Veil.Parser.Hail
{
    [TestFixture]
    internal class ConditionalOnModelPropertyTests : ParserTestBase<HailTemplateParser>
    {
        [Test]
        public void Should_parse_if_statement()
        {
            var template = Parse("Hello {{#if Conditional }} John{{/if}}", typeof(TestModel));
            AssertSyntaxTree(template, new ISyntaxTreeNode[] {
                WriteLiteralNode.String("Hello "),
                ConditionalOnModelPropertyNode.Create(typeof(TestModel), "Conditional",
                    new ISyntaxTreeNode[] { WriteLiteralNode.String(" John") }
                )
            });
        }

        private class TestModel
        {
            public bool Conditional { get; set; }
        }
    }
}