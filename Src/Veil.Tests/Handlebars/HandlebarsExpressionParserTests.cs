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
            result.ShouldDeepEqual(SyntaxTreeExpression.Property(typeof(Model), "Property"));
        }

        [Test]
        public void Should_parse_field()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "Field");
            result.ShouldDeepEqual(SyntaxTreeExpression.Field(typeof(Model), "Field"));
        }

        [Test]
        public void Should_parse_property_from_submodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "SubModel.SubProperty");
            result.ShouldDeepEqual(SyntaxTreeExpression.SubModel(SyntaxTreeExpression.Property(typeof(Model), "SubModel"), SyntaxTreeExpression.Property(typeof(SubModel), "SubProperty")));
        }

        [Test]
        public void Should_parse_field_from_subsubmodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "SubModel.SubSubModel.SubSubField");
            result.ShouldDeepEqual(SyntaxTreeExpression.SubModel(
                SyntaxTreeExpression.Property(typeof(Model), "SubModel"),
                SyntaxTreeExpression.SubModel(
                    SyntaxTreeExpression.Field(typeof(SubModel), "SubSubModel"),
                    SyntaxTreeExpression.Field(typeof(SubSubModel), "SubSubField"))
                )
            );
        }

        [Test]
        public void Should_parse_function_from_submodel()
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(Model)), "Function()");
            result.ShouldDeepEqual(SyntaxTreeExpression.Function(typeof(Model), "Function"));
        }

        [TestCase("this")]
        public void Should_parse_self_expression_node(string expression)
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(string)), expression);
            result.ShouldDeepEqual(SyntaxTreeExpression.Self(typeof(string)));
        }

        [TestCaseSource("LateBoundTestCases")]
        public void Should_parse_as_late_bound_when_model_type_is_not_known<T>(T model)
        {
            var result = HandlebarsExpressionParser.Parse(CreateScopes(typeof(T)), "Name");
            result.ShouldDeepEqual(SyntaxTreeExpression.LateBound("Name", false, ExpressionScope.CurrentModelOnStack));
        }

        [Test]
        public void Should_be_able_to_reference_parent_scopes_model()
        {
            var current = new { };
            var parent = new { Name = "" };
            var root = new { };

            var scopes = CreateScopes(root.GetType(), parent.GetType(), current.GetType());
            var result = HandlebarsExpressionParser.Parse(scopes, "../Name");
            result.ShouldDeepEqual(SyntaxTreeExpression.Property(parent.GetType(), "Name", ExpressionScope.ModelOfParentScope));
        }

        [TestCaseSource("CaseInsensitiveTests")]
        public void Should_match_case_insensitivity_correctly(string expression, ExpressionNode expectedResult)
        {
            var scopes = CreateScopes(typeof(CaseTestModel));
            var result = HandlebarsExpressionParser.Parse(scopes, expression);
            result.ShouldDeepEqual(expectedResult);
        }

        [TestCase("Foo")]
        [TestCase("Foo.Bar")]
        [TestCase("SubModel.Foo")]
        [TestCase("Property[]")]
        [TestCase("Property.foo()")]
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

        public object[] CaseInsensitiveTests()
        {
            return new object[] {
                new object[] { "UserName", SyntaxTreeExpression.Property(typeof(CaseTestModel), "UserName") },
                new object[] { "userName", SyntaxTreeExpression.Field(typeof(CaseTestModel), "userName") },
                new object[] { "username", SyntaxTreeExpression.Property(typeof(CaseTestModel), "UserName") },
                new object[] { "USERNAME", SyntaxTreeExpression.Property(typeof(CaseTestModel), "UserName") },
                new object[] { "FOO", SyntaxTreeExpression.Field(typeof(CaseTestModel), "foo") },
                new object[] { "function()", SyntaxTreeExpression.Function(typeof(CaseTestModel), "Function") },
                new object[] { "submodel.subproperty", SyntaxTreeExpression.SubModel(SyntaxTreeExpression.Property(typeof(CaseTestModel), "SubModel"), SyntaxTreeExpression.Property(typeof(SubModel), "SubProperty")) },
                new object[] { "submodel.subsubmodel.subsubfield", SyntaxTreeExpression.SubModel(SyntaxTreeExpression.Property(typeof(CaseTestModel), "SubModel"),SyntaxTreeExpression.SubModel(SyntaxTreeExpression.Field(typeof(SubModel), "SubSubModel"), SyntaxTreeExpression.Field(typeof(SubSubModel), "SubSubField")))}
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

        private class CaseTestModel
        {
            public string foo = "";

            public string userName = "";

            public string UserName { get { return userName; } }

            public string Function()
            {
                return "";
            }

            public SubModel SubModel { get; set; }
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