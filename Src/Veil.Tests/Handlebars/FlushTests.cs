using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    internal class FlushTests : ParserTestBase<HandlebarsParser>
    {
        [Test]
        public void Should_parse_flush()
        {
            var syntaxTree = Parse("{{#flush}}", typeof(object));
            AssertSyntaxTree(syntaxTree, SyntaxTree.Flush());
        }
    }
}