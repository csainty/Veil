using System.Collections.Generic;
using Veil.Parser;
using Xunit;

namespace Veil.Compiler
{
    
    public class IncludeTests : CompilerTestBase
    {
        [Fact]
        public void Should_throw_if_unable_to_load_template()
        {
            var model = new { Name = "Joe" };
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.Include("person", SyntaxTreeExpression.Self(model.GetType()))
            );

            Assert.Throws<VeilCompilerException>(() =>
            {
                ExecuteTemplate(template, model);
            });
        }

        [Fact]
        public void Should_render_include_with_same_model()
        {
            var model = new { Name = "Joe" };
            RegisterTemplate("person", SyntaxTree.Block(
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name"))
            ));
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.Include("person", SyntaxTreeExpression.Self(model.GetType()))
            );

            var result = ExecuteTemplate(template, model);
            Assert.Equal("Hello Joe", result);
        }

        [Fact]
        public void Should_render_include_with_same_sub_model()
        {
            var model = new { Name = "Joe", Company = new { Name = "Foo" } };
            RegisterTemplate("company", SyntaxTree.Block(
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.Company.GetType(), "Name"))
            ));
            RegisterTemplate("person", SyntaxTree.Block(
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name"))
            ));
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Welcome from "),
                SyntaxTree.Include("company", SyntaxTreeExpression.Property(model.GetType(), "Company")),
                SyntaxTree.WriteString(" the amazing "),
                SyntaxTree.Include("person", SyntaxTreeExpression.Self(model.GetType())),
                SyntaxTree.WriteString(".")
            );

            var result = ExecuteTemplate(template, model);
            Assert.Equal("Welcome from Foo the amazing Joe.", result);
        }

        [Fact]
        public void Root_model_of_template_should_be_the_model_executed_against()
        {
            var model = new { Name = "Joe", Company = new { Name = "Foo", Departments = new[] { "IT", "Admin" } } };
            RegisterTemplate("company", SyntaxTree.Block(
                SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.Company.GetType(), "Name")),
                SyntaxTree.WriteString(" - "),
                SyntaxTree.Iterate(SyntaxTreeExpression.Property(model.Company.GetType(), "Departments"), SyntaxTree.Block(
                    SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.Company.GetType(), "Name", ExpressionScope.RootModel))
                ))
            ));
            var template = SyntaxTree.Block(
                SyntaxTree.Include("company", SyntaxTreeExpression.Property(model.GetType(), "Company"))
            );

            var result = ExecuteTemplate(template, model);
            Assert.Equal("Foo - FooFoo", result);
        }

        [Fact]
        public void Should_render_include_with_untyped_model()
        {
            var model = new Dictionary<string, object>();
            model.Add("Name", "Joe");

            RegisterTemplate("person", SyntaxTree.Block(
                SyntaxTree.WriteExpression(SyntaxTreeExpression.LateBound("Name"))
            ));
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.Include("person", SyntaxTreeExpression.Self(model.GetType()))
            );

            var result = ExecuteTemplate(template, model);
            Assert.Equal("Hello Joe", result);
        }

        [Fact]
        public void Should_render_include_with_untyped_sub_model()
        {
            var model = new Dictionary<string, object>();
            model.Add("Person", new { Name = "Joe" });

            RegisterTemplate("person", SyntaxTree.Block(
                SyntaxTree.WriteExpression(SyntaxTreeExpression.LateBound("Name"))
            ));
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.Include("person", SyntaxTreeExpression.LateBound("Person"))
            );

            var result = ExecuteTemplate(template, model);
            Assert.Equal("Hello Joe", result);
        }
    }
}