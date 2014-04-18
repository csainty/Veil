using DeepEqual.Syntax;
using NUnit.Framework;

namespace Veil.Handlebars
{
    [TestFixture]
    public class HandlebarsExpressionParerTests
    {
        [Test]
        public void Should_parse_property()
        {
            var result = HandlebarsExpressionParser.Parse(typeof(Model), "Property");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.ModelProperty(typeof(Model), "Property"));
        }

        [Test]
        public void Should_parse_field()
        {
            var result = HandlebarsExpressionParser.Parse(typeof(Model), "Field");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.ModelField(typeof(Model), "Field"));
        }

        [Test]
        public void Should_parse_property_from_submodel()
        {
            var result = HandlebarsExpressionParser.Parse(typeof(Model), "SubModel.SubProperty");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.ModelSubModel(SyntaxTreeNode.ExpressionNode.ModelProperty(typeof(Model), "SubModel"), SyntaxTreeNode.ExpressionNode.ModelProperty(typeof(SubModel), "SubProperty")));
        }

        [Test]
        public void Should_parse_field_from_subsubmodel()
        {
            var result = HandlebarsExpressionParser.Parse(typeof(Model), "SubModel.SubSubModel.SubSubField");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.ModelSubModel(
                SyntaxTreeNode.ExpressionNode.ModelProperty(typeof(Model), "SubModel"),
                SyntaxTreeNode.ExpressionNode.ModelSubModel(
                    SyntaxTreeNode.ExpressionNode.ModelField(typeof(SubModel), "SubSubModel"),
                    SyntaxTreeNode.ExpressionNode.ModelField(typeof(SubSubModel), "SubSubField"))
                )
            );
        }

        [Test]
        public void Should_parse_function_from_submodel()
        {
            var result = HandlebarsExpressionParser.Parse(typeof(Model), "Function()");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.ModelFunction(typeof(Model), "Function"));
        }

        [TestCase("this")]
        public void Should_parse_self_expression_node(string expression)
        {
            var result = HandlebarsExpressionParser.Parse(typeof(string), expression);
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.Self(typeof(string)));
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
                HandlebarsExpressionParser.Parse(typeof(Model), expression);
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