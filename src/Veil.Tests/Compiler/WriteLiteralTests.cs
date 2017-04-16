using Veil.Parser;
using Veil.Parser.Nodes;
using Xunit;

namespace Veil.Compiler
{
    public class WriteLiteralTests : CompilerTestBase
    {
        [InlineData("Hello World", "Hello World")]
        [Theory]
        public void Should_output_literal(string literal, string expectedResult)
        {
            var template = SyntaxTree.Block(new WriteLiteralNode { LiteralContent = literal });
            var result = ExecuteTemplate(template, new { });
            Assert.Equal(expectedResult, result);
        }
    }
}