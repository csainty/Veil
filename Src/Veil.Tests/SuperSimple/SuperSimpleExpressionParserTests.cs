using DeepEqual.Syntax;
using NUnit.Framework;

namespace Veil.SuperSimple
{
    [TestFixture]
    internal class SuperSimpleExpressionParserTests
    {
        [Test]
        public void Should_parse_model_keywords_as_self_expression()
        {
            var model = new { };
            var result = SuperSimpleExpressionParser.Parse(model.GetType(), "Model");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.Self(model.GetType()));
        }

        [Test]
        public void Should_parse_current_keyword_as_self_expression()
        {
            var model = new { };
            var result = SuperSimpleExpressionParser.Parse(model.GetType(), "Current");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.Self(model.GetType()));
        }

        [Test]
        public void Should_parse_model_dot_property_as_proeprty_expression()
        {
            var model = new { Name = "foo" };
            var result = SuperSimpleExpressionParser.Parse(model.GetType(), "Model.Name");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Name"));
        }

        [TestCase("Model.Wrong")]
        public void Should_return_null_for_invalid_expressions(string expression)
        {
            var model = new { };
            var result = SuperSimpleExpressionParser.Parse(model.GetType(), expression);
            Assert.That(result, Is.Null);
        }
    }
}