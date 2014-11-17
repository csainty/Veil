using NUnit.Framework;
using Veil.Parser;

namespace Veil.Expressions
{
    [TestFixture]
    public class FunctionCallExpressionNodeTests
    {
        [Test]
        public void Should_choose_parameter_less_version_of_function()
        {
            Assert.DoesNotThrow(() =>
            {
                var result = SyntaxTreeExpression.Function(typeof(string), "ToString");
            });
        }
    }
}