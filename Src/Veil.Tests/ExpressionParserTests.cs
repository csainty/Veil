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
        public void Should_parse_property_from_submodel()
        {
            var result = ExpressionParser.Parse(typeof(Model), "SubModel.SubProperty");
            result.ShouldDeepEqual(SubModelExpressionNode.Create(ModelPropertyExpressionNode.Create(typeof(Model), "SubModel"), ModelPropertyExpressionNode.Create(typeof(SubModel), "SubProperty")));
        }

        [Test]
        public void Should_parse_field_from_subsubmodel()
        {
            var result = ExpressionParser.Parse(typeof(Model), "SubModel.SubSubModel.SubSubField");
            result.ShouldDeepEqual(SubModelExpressionNode.Create(
                ModelPropertyExpressionNode.Create(typeof(Model), "SubModel"),
                SubModelExpressionNode.Create(
                    ModelFieldExpressionNode.Create(typeof(SubModel), "SubSubModel"),
                    ModelFieldExpressionNode.Create(typeof(SubSubModel), "SubSubField"))
                )
            );
        }

        [TestCase("Foo")]
        [TestCase("Foo.Bar")]
        [TestCase("SubModel.Foo")]
        [TestCase("property")]
        [TestCase("field")]
        [TestCase("Property()")]
        [TestCase("Property[]")]
        [TestCase("Property.ToString()")]
        public void Should_throw_if_expression_cant_be_parsed(string expression)
        {
            Assert.Throws<VeilParserException>(() =>
            {
                ExpressionParser.Parse(typeof(Model), expression);
            });
        }

        private class Model
        {
            public bool Property { get; set; }

            public bool Field = false;

            public SubModel SubModel { get; set; }
        }

        private class SubModel
        {
            public bool SubProperty { get; set; }

            public SubSubModel SubSubModel = null;
        }

        private class SubSubModel
        {
            public string SubSubField = "";
        }
    }
}