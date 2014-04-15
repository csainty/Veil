using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class WriteLiteralTests : CompilerTestBase
    {
        [SetCulture("en-US")]
        [TestCase("Hello World", "Hello World")]
        [TestCase(1, "1")]
        [TestCase(1.1, "1.1")]
        [TestCase(1.1F, "1.1")]
        [TestCase(100L, "100")]
        [TestCase(2U, "2")]
        [TestCase(2UL, "2")]
        public void Should_output_literals(object literal, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(new SyntaxTreeNode.WriteLiteralNode { LiteralContent = literal, LiteralType = literal.GetType() });
            var result = ExecuteTemplate(template, new { });
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Should_throw_when_literal_type_is_unknown()
        {
            var template = SyntaxTreeNode.Block(new SyntaxTreeNode.WriteLiteralNode { LiteralContent = new object(), LiteralType = typeof(object) });
            Assert.Throws<VeilCompilerException>(() =>
            {
                ExecuteTemplate(template, new { });
            });
        }
    }
}