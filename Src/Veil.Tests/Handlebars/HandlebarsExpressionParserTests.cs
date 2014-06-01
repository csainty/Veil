using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using DeepEqual.Syntax;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.Handlebars
{
    [TestFixture]
    public class HandlebarsExpressionParserTests
    {
        [Test]
        public void Should_parse_property()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "Property");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.Property(typeof(Model), "Property"));
        }

        [Test]
        public void Should_parse_field()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "Field");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.Field(typeof(Model), "Field"));
        }

        [Test]
        public void Should_parse_property_from_submodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "SubModel.SubProperty");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.SubModel(SyntaxTreeNode.ExpressionNode.Property(typeof(Model), "SubModel"), SyntaxTreeNode.ExpressionNode.Property(typeof(SubModel), "SubProperty")));
        }

        [Test]
        public void Should_parse_field_from_subsubmodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "SubModel.SubSubModel.SubSubField");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.SubModel(
                SyntaxTreeNode.ExpressionNode.Property(typeof(Model), "SubModel"),
                SyntaxTreeNode.ExpressionNode.SubModel(
                    SyntaxTreeNode.ExpressionNode.Field(typeof(SubModel), "SubSubModel"),
                    SyntaxTreeNode.ExpressionNode.Field(typeof(SubSubModel), "SubSubField"))
                )
            );
        }

        [Test]
        public void Should_parse_function_from_submodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "Function()");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.Function(typeof(Model), "Function"));
        }

        [TestCase("this")]
        public void Should_parse_self_expression_node(string expression)
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(string)), expression);
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.Self(typeof(string)));
        }

        [TestCaseSource("LateBoundTestCases")]
        public void Should_parse_as_late_bound_when_model_type_is_not_known<T>(T model)
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(T)), "Name");
            result.ShouldDeepEqual(SyntaxTreeNode.ExpressionNode.LateBound("Name", SyntaxTreeNode.ExpressionScope.CurrentModelOnStack));
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
                HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), expression);
            });
        }

        public object[] LateBoundTestCases()
        {
            return new object[] {
                new object[] { new object() },
                new object[] { new Dictionary<string, object>() },
                new object[] { new ExpandoObject() }
            };
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

        private static LinkedList<HandlebarsParser.ParserScope> CreateScopes(params Type[] models)
        {
            return new LinkedList<HandlebarsParser.ParserScope>(models.Select(x => new HandlebarsParser.ParserScope { Block = SyntaxTreeNode.Block(), ModelInScope = x }));
        }
    }
}