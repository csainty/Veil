using Veil.Parser;
using Xunit;

namespace Veil.Expressions
{
    public class FunctionCallExpressionNodeTests
    {
        [Fact]
        public void Should_choose_parameter_less_version_of_function()
        {
            var result = SyntaxTreeExpression.Function(typeof(string), "ToString");
        }
    }
}