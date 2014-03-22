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
                new WriteLiteralNode { LiteralType = typeof(string), LiteralContent = "Hello " },
                new WriteModelPropertyNode { ModelProperty = model.GetType().GetProperty("Name") },
                new WriteLiteralNode { LiteralType = typeof(string), LiteralContent = "!" }
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello World!"));
        }
    }
}