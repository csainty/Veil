using System.Collections.Generic;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class IterationTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
        public void Should_parse_each_block()
        {
            var result = Parse("{{#each Items }}{{ this }}{{/each}}", typeof(TestModel));
            AssertSyntaxTree(result,
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(typeof(TestModel), "Items"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string)), true)
                    )
                )
            );
        }

        [Test]
        public void Should_be_able_to_reference_parent_scope_from_within_each_block()
        {
            var result = Parse("{{#each Items }}{{ ../Prefix }}{{ this }}{{/each}}", typeof(TestModel));
            AssertSyntaxTree(result,
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(typeof(TestModel), "Items"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Prefix", ExpressionScope.ModelOfParentScope), true),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string)), true)
                    )
                )
            );
        }

        [Test]
        public void Should_parse_else_block_of_each()
        {
            var result = Parse("{{#each Items }}Foo{{else}}Bar{{/each}}", typeof(TestModel));
            AssertSyntaxTree(result,
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(typeof(TestModel), "Items"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("Foo")
                    ),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("Bar")
                    )
                )
            );
        }

        [TestCase("Hello {{#each Items}} There")]
        [TestCase("Hello {{#each Items}} There {{else}}")]
        [TestCase("Hello {{/each}} There")]
        [TestCase("Hello {{#each Items}} There{{/each}}{{/each}}")]
        [TestCase("Hello {{else}} Foo")]
        [TestCase("Hello {{#each Items}} There{{/each}}{{else}}")]
        public void Should_throw_if_block_not_open_and_closed_consistently(string template)
        {
            Assert.Throws<VeilParserException>(() =>
            {
                Parse(template, typeof(TestModel));
            });
        }

        private class TestModel
        {
            public string Prefix { get; set; }

            public IEnumerable<string> Items { get; set; }
        }
    }
}