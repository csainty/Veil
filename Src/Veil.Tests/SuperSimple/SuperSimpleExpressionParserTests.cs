using System;
using System.Collections.Generic;
using DeepEqual.Syntax;
using NUnit.Framework;
using E = Veil.SyntaxTreeNode.ExpressionNode;

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
            result.ShouldDeepEqual(E.Self(model.GetType(), SyntaxTreeNode.ExpressionScope.RootModel));
        }

        [Test]
        public void Should_parse_current_keyword_as_self_expression()
        {
            var model = new { };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Current");
            result.ShouldDeepEqual(E.Self(model.GetType()));
        }

        [Test]
        public void Should_parse_model_dot_property_as_proeprty_expression()
        {
            var model = new { Name = "foo" };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Model.Name");
            result.ShouldDeepEqual(E.Property(model.GetType(), "Name", SyntaxTreeNode.ExpressionScope.RootModel));
        }

        [Test]
        public void Should_parse_hasitems_expression()
        {
            var model = new { Items = new[] { 1, 2, 3 } };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "HasItems");
            result.ShouldDeepEqual(E.HasItems(E.Property(model.GetType(), "Items")));
        }

        [Test]
        public void Should_give_precedence_to_property_over_hasitems()
        {
            var model = new { Items = new[] { 1, 2, 3 }, HasItems = true };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "HasItems");
            result.ShouldDeepEqual(E.Property(model.GetType(), "HasItems"));
        }

        [Test]
        public void Should_parse_sub_model_expression_from_root_model()
        {
            var model = new { User = new { Name = "Bob" } };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Model.User.Name");
            result.ShouldDeepEqual(E.SubModel(
                E.Property(model.GetType(), "User", SyntaxTreeNode.ExpressionScope.RootModel),
                E.Property(model.User.GetType(), "Name")
            ));
        }

        [Test]
        public void Should_parse_sub_model_expression_from_current_model()
        {
            var model = new { User = new { Name = "Bob" } };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Current.User.Name");
            result.ShouldDeepEqual(E.SubModel(
                E.Property(model.GetType(), "User"),
                E.Property(model.User.GetType(), "Name")
            ));
        }

        [Test]
        public void Should_parse_multiple_sub_model_expressions()
        {
            var model = new { User = new { Department = new { Company = new { Name = "Foo" } } } };
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(model.GetType()), "Model.User.Department.Company.Name");
            result.ShouldDeepEqual(E.SubModel(
                E.Property(model.GetType(), "User", SyntaxTreeNode.ExpressionScope.RootModel),
                E.SubModel(
                    E.Property(model.User.GetType(), "Department"),
                    E.SubModel(
                        E.Property(model.User.Department.GetType(), "Company"),
                        E.Property(model.User.Department.Company.GetType(), "Name")
                    )
                )
            ));
        }

        [Test]
        public void Should_parse_field_references()
        {
            var result = SuperSimpleExpressionParser.Parse(CreateScopes(typeof(ViewModel)), "Model.FieldName");
            result.ShouldDeepEqual(E.Field(typeof(ViewModel), "FieldName", SyntaxTreeNode.ExpressionScope.RootModel));
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

        private LinkedList<SuperSimpleParser.ParserScope> CreateScopes(Type rootScope, Type currentScope = null)
        {
            var scopes = new LinkedList<SuperSimpleParser.ParserScope>();
            scopes.AddFirst(new SuperSimpleParser.ParserScope { ModelType = rootScope });
            if (currentScope != null)
            {
                scopes.AddFirst(new SuperSimpleParser.ParserScope { ModelType = currentScope });
            }
            return scopes;
        }

        private class ViewModel
        {
            public bool FieldName = true;
        }
    }
}