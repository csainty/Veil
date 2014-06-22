using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    [TestFixture]
    internal class ScopedBlockNodeTests : CompilerTestBase
    {
        [Test]
        public void Should_change_scope_in_a_scope_block()
        {
            var model = new { User = new { Name = "Joe" }, Foo = "Bar" };
            var result = ExecuteTemplate(SyntaxTree.Block(
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.ScopeBlock(
                    Expression.Property(model.GetType(), "User"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(Expression.Property(model.User.GetType(), "Name"))
                    )
                ),
                SyntaxTree.WriteExpression(Expression.Property(model.GetType(), "Foo"))
            ), model);
            Assert.That(result, Is.EqualTo("Hello JoeBar"));
        }
    }
}