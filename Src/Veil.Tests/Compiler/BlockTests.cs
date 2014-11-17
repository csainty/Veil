using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    [TestFixture]
    internal class BlockTests : CompilerTestBase
    {
        [Test]
        public void Should_emit_all_nodes_in_block()
        {
            var model = new { Name = "World" };
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name")),
                SyntaxTree.WriteString("!")
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello World!"));
        }
    }
}