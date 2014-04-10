using DeepEqual.Syntax;
using NUnit.Framework;
using Veil.Parser;

namespace Veil
{
    [TestFixture]
    internal class ExpressionParserTests
    {
        [Test]
        public void Should_parse_property()
        {
            var result = ExpressionParser.Parse(typeof(Model), "Property");
            result.ShouldDeepEqual(ModelPropertyExpressionNode.Create(typeof(Model), "Property"));
        }

        [Test]
        public void Should_parse_field()
        {
            var result = ExpressionParser.Parse(typeof(Model), "Field");
            result.ShouldDeepEqual(ModelFieldExpressionNode.Create(typeof(Model), "Field"));
        }

        [Test]
        public void Should_throw_if_expression_cant_be_parsed()
        {
            Assert.Throws<VeilParserException>(() =>
            {
                ExpressionParser.Parse(typeof(Model), "Foo.Bar");
            });
        }

        private class Model
        {
            public bool Property { get; set; }

            public bool Field;
        }
    }
}