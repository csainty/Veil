using NUnit.Framework;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    [TestFixture]
    internal class WriteLiteralTests : CompilerTestBase
    {
        [TestCase("Hello World", "Hello World")]
        public void Should_output_literal(string literal, string expectedResult)
        {
            var template = SyntaxTree.Block(new WriteLiteralNode { LiteralContent = literal });
            var result = ExecuteTemplate(template, new { });
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}