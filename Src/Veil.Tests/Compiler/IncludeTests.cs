using NUnit.Framework;
using E = Veil.SyntaxTreeNode.ExpressionNode;
using S = Veil.SyntaxTreeNode;

namespace Veil.Compiler
{
    [TestFixture]
    internal class IncludeTests : CompilerTestBase
    {
        [Test]
        public void Should_throw_if_unable_to_load_template()
        {
            var model = new { Name = "Joe" };
            var template = S.Block(
                S.WriteString("Hello "),
                S.Include("person", E.Self(model.GetType()))
            );

            Assert.Throws<VeilCompilerException>(() =>
            {
                ExecuteTemplate(template, model);
            });
        }

        [Test]
        public void Should_render_include_with_same_model()
        {
            var model = new { Name = "Joe" };
            RegisterTemplate("person", S.Block(
                S.WriteExpression(E.Property(model.GetType(), "Name"))
            ));
            var template = S.Block(
                S.WriteString("Hello "),
                S.Include("person", E.Self(model.GetType()))
            );

            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello Joe"));
        }

        [Test]
        public void Should_render_include_with_same_sub_model()
        {
            var model = new { Name = "Joe", Company = new { Name = "Foo" } };
            RegisterTemplate("company", S.Block(
                S.WriteExpression(E.Property(model.Company.GetType(), "Name"))
            ));
            RegisterTemplate("person", S.Block(
                S.WriteExpression(E.Property(model.GetType(), "Name"))
            ));
            var template = S.Block(
                S.WriteString("Welcome from "),
                S.Include("company", E.Property(model.GetType(), "Company")),
                S.WriteString(" the amazing "),
                S.Include("person", E.Self(model.GetType())),
                S.WriteString(".")
            );

            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Welcome from Foo the amazing Joe."));
        }

        [Test]
        public void Root_model_of_template_should_be_the_model_executed_against()
        {
            var model = new { Name = "Joe", Company = new { Name = "Foo", Departments = new[] { "IT", "Admin" } } };
            RegisterTemplate("company", S.Block(
                S.WriteExpression(E.Property(model.Company.GetType(), "Name")),
                S.WriteString(" - "),
                S.Iterate(E.Property(model.Company.GetType(), "Departments"), S.Block(
                    S.WriteExpression(E.Property(model.Company.GetType(), "Name", S.ExpressionScope.RootModel))
                ))
            ));
            var template = S.Block(
                S.Include("company", E.Property(model.GetType(), "Company"))
            );

            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Foo - FooFoo"));
        }
    }
}