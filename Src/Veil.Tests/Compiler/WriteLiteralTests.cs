using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    [TestFixture]
    internal class WriteLiteralTests : CompilerTestBase
    {
        [TestCase("Hello World", "Hello World")]
        public void Should_output_literal(string literal, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(new SyntaxTreeNode.WriteLiteralNode { LiteralContent = literal });
            var result = ExecuteTemplate(template, new { });
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}