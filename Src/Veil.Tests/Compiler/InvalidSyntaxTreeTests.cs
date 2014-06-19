using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    [TestFixture]
    internal class InvalidSyntaxTreeTests : CompilerTestBase
    {
        [Test]
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