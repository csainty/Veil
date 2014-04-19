using System.Collections.Generic;
using NUnit.Framework;

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
                SyntaxTreeNode.Iterate(
                    SyntaxTreeNode.ExpressionNode.Property(typeof(TestModel), "Items"),
                    SyntaxTreeNode.Block(
                        SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Self(typeof(string)))
                    )
                )
            );
        }

        private class TestModel
        {
            public IEnumerable<string> Items { get; set; }
        }
    }
}