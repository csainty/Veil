using System;
using System.Collections.Generic;
using System.Dynamic;
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
            result.ShouldDeepEqual(Expression.Property(typeof(Model), "Property"));
        }

        [Test]
        public void Should_parse_field()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "Field");
            result.ShouldDeepEqual(Expression.Field(typeof(Model), "Field"));
        }

        [Test]
        public void Should_parse_property_from_submodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "SubModel.SubProperty");
            result.ShouldDeepEqual(Expression.SubModel(Expression.Property(typeof(Model), "SubModel"), Expression.Property(typeof(SubModel), "SubProperty")));
        }

        [Test]
        public void Should_parse_field_from_subsubmodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "SubModel.SubSubModel.SubSubField");
            result.ShouldDeepEqual(Expression.SubModel(
                Expression.Property(typeof(Model), "SubModel"),
                Expression.SubModel(
                    Expression.Field(typeof(SubModel), "SubSubModel"),
                    Expression.Field(typeof(SubSubModel), "SubSubField"))
                )
            );
        }

        [Test]
        public void Should_parse_function_from_submodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "Function()");
            result.ShouldDeepEqual(Expression.Function(typeof(Model), "Function"));
        }

        [TestCase("this")]
        public void Should_parse_self_expression_node(string expression)
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(string)), expression);
            result.ShouldDeepEqual(Expression.Self(typeof(string)));
        }

        [TestCaseSource("LateBoundTestCases")]
        public void Should_parse_as_late_bound_when_model_type_is_not_known<T>(T model)
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(T)), "Name");
            result.ShouldDeepEqual(Expression.LateBound("Name", ExpressionScope.CurrentModelOnStack));
        }

        [Test]
        public void Should_be_able_to_reference_parent_scopes_model()
        {
            var current = new { };
            var parent = new { Name = "" };
            var root = new { };

            var scopes = CreateScopes(root.GetType(), parent.GetType(), current.GetType());
            var result = HandlebarsExpressionParser.Parse(scopes, "../Name");
            result.ShouldDeepEqual(Expression.Property(parent.GetType(), "Name", ExpressionScope.ModelOfParentScope));
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

        private static HandlebarsBlockStack CreateScopes(params Type[] modelTypes)
        {
            var scopes = new HandlebarsBlockStack();
            foreach (var modelType in modelTypes)
            {
                scopes.PushBlock(new HandlebarsParserBlock { Block = SyntaxTree.Block(), ModelInScope = modelType });
            }
            return scopes;
        }
    }
}