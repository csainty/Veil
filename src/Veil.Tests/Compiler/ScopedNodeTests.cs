using Veil.Parser;
using Xunit;

namespace Veil.Compiler
{
    public class ScopedNodeTests : CompilerTestBase
    {
        [Fact]
        public void Should_change_scope_in_a_scope_block()
        {
            var model = new { User = new { Name = "Joe" }, Foo = "Bar" };
            var result = ExecuteTemplate(SyntaxTree.Block(
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.ScopeNode(
                    SyntaxTreeExpression.Property(model.GetType(), "User"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.User.GetType(), "Name"))
                    )
                ),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Foo"))
            ), model);
            Assert.Equal("Hello JoeBar", result);
        }
    }
}