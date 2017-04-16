using Veil.Parser;
using Xunit;

namespace Veil.Compiler
{
    public class InvalidSyntaxTreeTests : CompilerTestBase
    {
        [Fact]
        public void Should_throw_when_invalid_syntax_node_found()
        {
            var template = SyntaxTree.Block(new InvalidSyntaxTreeNode());
            Assert.Throws<VeilCompilerException>(() =>
            {
                ExecuteTemplate(template, new { });
            });
        }

        private class InvalidSyntaxTreeNode : SyntaxTreeNode { }
    }
}