using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class TemplateRootNodeTests : CompilerTestBase
    {
        [Test]
        public void Should_emit_all_nodes_in_syntax_tree()
        {
            var model = new { Name = "World" };
            var template = CreateTemplate(
                WriteLiteralNode.String("Hello "),
                WriteModelPropertyNode.Create(model.GetType(), "Name"),
                WriteLiteralNode.String("!")
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello World!"));
        }
    }
}