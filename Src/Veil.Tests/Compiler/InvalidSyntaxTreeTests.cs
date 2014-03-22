using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class InvalidSyntaxTreeTests : CompilerTestBase
    {
        [Test]
        public void Should_throw_when_invalid_syntax_node_found()
        {
            var template = CreateTemplate(new InvalidSyntaxTreeNode());
            Assert.Throws<VeilCompilerException>(() =>
            {
                ExecuteTemplate(template, new { });
            });
        }

        private class InvalidSyntaxTreeNode : ISyntaxTreeNode { }
    }
}