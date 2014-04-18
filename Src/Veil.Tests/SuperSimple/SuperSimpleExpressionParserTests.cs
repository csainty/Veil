using DeepEqual.Syntax;
using NUnit.Framework;

namespace Veil.SuperSimple
{
    [TestFixture]
    internal class SuperSimpleExpressionParserTests
    {
        [Test]
        public void Should_parse_Model_keyword_as_self_expression()
        {
            var model = new { };
            var result = SuperSimpleExpressionParser.Parse(model.GetType(), "Model");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.Self(model.GetType()));
        }
    }
}