using System;
using System.Collections.Generic;
using System.Dynamic;
using DeepEqual.Syntax;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.SuperSimple
{
    [TestFixture]
    internal class SuperSimpleExpressionParserTests
    {
        [Test]
        public void Should_parse_model_keywords_as_self_expression_scoped_to_root()
        {
            var model = new { };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Model");
            result.ShouldDeepEqual(Expression.Self(model.GetType(), ExpressionScope.RootModel));
        }

        [Test]
        public void Should_parse_current_keyword_as_self_expression()
        {
            var model = new { };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Current");
            result.ShouldDeepEqual(Expression.Self(model.GetType()));
        }

        [Test]
        public void Should_parse_model_dot_property_as_proeprty_expression()
        {
            var model = new { Name = "foo" };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Model.Name");
            result.ShouldDeepEqual(Expression.Property(model.GetType(), "Name", ExpressionScope.RootModel));
        }

        [Test]
        public void Should_parse_hasitems_expression()
        {
            var model = new { Items = new[] { 1, 2, 3 } };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "HasItems");
            result.ShouldDeepEqual(Expression.HasItems(Expression.Property(model.GetType(), "Items")));
        }

        [Test]
        public void Should_give_precedence_to_property_over_hasitems()
        {
            var model = new { Items = new[] { 1, 2, 3 }, HasItems = true };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "HasItems");
            result.ShouldDeepEqual(Expression.Property(model.GetType(), "HasItems"));
        }

        [Test]
        public void Should_parse_sub_model_expression_from_root_model()
        {
            var model = new { User = new { Name = "Bob" } };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Model.User.Name");
            result.ShouldDeepEqual(Expression.SubModel(
                Expression.Property(model.GetType(), "User", ExpressionScope.RootModel),
                Expression.Property(model.User.GetType(), "Name")
            ));
        }

        [Test]
        public void Should_parse_sub_model_expression_from_current_model()
        {
            var model = new { User = new { Name = "Bob" } };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Current.User.Name");
            result.ShouldDeepEqual(Expression.SubModel(
                Expression.Property(model.GetType(), "User"),
                Expression.Property(model.User.GetType(), "Name")
            ));
        }

        [Test]
        public void Should_parse_multiple_sub_model_expressions()
        {
            var model = new { User = new { Department = new { Company = new { Name = "Foo" } } } };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Model.User.Department.Company.Name");
            result.ShouldDeepEqual(Expression.SubModel(
                Expression.Property(model.GetType(), "User", ExpressionScope.RootModel),
                Expression.SubModel(
                    Expression.Property(model.User.GetType(), "Department"),
                    Expression.SubModel(
                        Expression.Property(model.User.Department.GetType(), "Company"),
                        Expression.Property(model.User.Department.Company.GetType(), "Name")
                    )
                )
            ));
        }

        [Test]
        public void Should_parse_field_references()
        {
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(typeof(ViewModel)), "Model.FieldName");
            result.ShouldDeepEqual(Expression.Field(typeof(ViewModel), "FieldName", ExpressionScope.RootModel));
        }

        [TestCaseSource("LateBoundTestCases")]
        public void Should_parse_as_late_bound_when_model_type_is_not_known<T>(T model)
        {
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(typeof(T)), "Model.Name");
            result.ShouldDeepEqual(Expression.LateBound("Name", true, ExpressionScope.RootModel));
        }

        [Test]
        public void Should_preference_late_binding_over_Has_prefix()
        {
            var model = new { HasItems = true, Items = new string[0] };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(typeof(object)), "Model.HasItems");
            result.ShouldDeepEqual(Expression.LateBound("HasItems", true, ExpressionScope.RootModel));
        }

        [TestCase("Model.Wrong")]
        [TestCase("Model.name")]
        public void Should_throw_for_invalid_expressions(string expression)
        {
            var model = new { Name = "foo" };
            Assert.Throws<VeilParserException>(() =>
            {
                SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), expression);
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

        private LinkedList<SuperSimpleTemplateParserScope> CreateScopes(Type rootScope, Type currentScope = null)
        {
            var scopes = new LinkedList<SuperSimpleTemplateParserScope>();
            scopes.AddFirst(new SuperSimpleTemplateParserScope { ModelType = rootScope });
            if (currentScope != null)
            {
                scopes.AddFirst(new SuperSimpleTemplateParserScope { ModelType = currentScope });
            }
            return scopes;
        }

        private class ViewModel
        {
            public bool FieldName = true;
        }
    }
}