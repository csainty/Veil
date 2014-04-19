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
                SyntaxTreeNode.WriteString("Hello "),
                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Name")),
                SyntaxTreeNode.WriteString("!")
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello World!"));
        }
    }
}