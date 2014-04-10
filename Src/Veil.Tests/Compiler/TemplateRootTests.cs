using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class TemplateRootTests : CompilerTestBase
    {
        [Test]
        public void Should_emit_all_nodes_in_syntax_tree()
        {
            var model = new { Name = "World" };
            var template = CreateTemplate(
                WriteLiteralNode.String("Hello "),
                WriteModelExpressionNode.Create(model.GetType(), "Name"),
                WriteLiteralNode.String("!")
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello World!"));
        }
    }
}