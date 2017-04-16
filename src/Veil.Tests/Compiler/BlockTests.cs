using Veil.Parser;
using Xunit;

namespace Veil.Compiler
{
    public class BlockTests : CompilerTestBase
    {
        [Fact]
        public void Should_emit_all_nodes_in_block()
        {
            var model = new { Name = "World" };
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name")),
                SyntaxTree.WriteString("!")
            );
            var result = ExecuteTemplate(template, model);
            Assert.Equal("Hello World!", result);
        }
    }
}