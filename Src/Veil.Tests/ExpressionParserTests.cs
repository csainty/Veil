using DeepEqual.Syntax;
using NUnit.Framework;

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

        [Test]
        public void Should_parse_function_from_submodel()
        {
            var result = ExpressionParser.Parse(typeof(Model), "Function()");
            result.ShouldDeepEqual(FunctionCallExpressionNode.Create(typeof(Model), "Function"));
        }

        [TestCase("this")]
        public void Should_parse_self_expression_node(string expression)
        {
            var result = ExpressionParser.Parse(typeof(Model), expression);
            result.ShouldDeepEqual(SelfExpressionNode.Create(typeof(Model)));
        }

        [TestCase("Foo")]
        [TestCase("Foo.Bar")]
        [TestCase("SubModel.Foo")]
        [TestCase("property")]
        [TestCase("field")]
        [TestCase("Property[]")]
        [TestCase("function()")]
        [TestCase("Property.toString()")]
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

            public string Function()
            {
                return "Func";
            }
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