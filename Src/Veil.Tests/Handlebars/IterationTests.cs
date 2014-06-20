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
                    Expression.Property(typeof(TestModel), "Items"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(Expression.Self(typeof(string)), true)
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
                    Expression.Property(typeof(TestModel), "Items"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(Expression.Property(typeof(TestModel), "Prefix", ExpressionScope.ModelOfParentScope), true),
                        SyntaxTree.WriteExpression(Expression.Self(typeof(string)), true)
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
                    Expression.Property(typeof(TestModel), "Items"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("Foo")
                    ),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("Bar")
                    )
                )
            );
        }

        private class TestModel
        {
            public string Prefix { get; set; }

            public IEnumerable<string> Items { get; set; }
        }
    }
}