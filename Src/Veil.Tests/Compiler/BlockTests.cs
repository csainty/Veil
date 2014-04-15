using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class BlockTests : CompilerTestBase
    {
        [Test]
        public void Should_emit_all_nodes_in_block()
        {
            var model = new { Name = "World" };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.StringLiteral("Hello "),
                WriteModelExpressionNode.Create(model.GetType(), "Name"),
                SyntaxTreeNode.StringLiteral("!")
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello World!"));
        }
    }
}