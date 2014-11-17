using System.Collections.Generic;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    [TestFixture]
    internal class ExtendTests : CompilerTestBase
    {
        [Test]
        public void Should_be_able_to_extend_a_template()
        {
            var model = new { Name = "Bob" };
            RegisterTemplate("master", SyntaxTree.Block(
                SyntaxTree.WriteString("<html><head>"),
                SyntaxTree.Override("head"),
                SyntaxTree.WriteString("</head></html>")
            ));
            var template = SyntaxTree.Extend("master", new Dictionary<string, SyntaxTreeNode> {
                {
                    "head",
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("<title>"),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name")),
                        SyntaxTree.WriteString("</title>")
                    )
                }
            });
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo("<html><head><title>Bob</title></head></html>"));
        }

        [Test]
        public void Should_not_throw_if_optional_override_is_missing()
        {
            var model = new { };
            RegisterTemplate("master", SyntaxTree.Block(SyntaxTree.WriteString("Hello"), SyntaxTree.Override("foo", true)));
            var template = SyntaxTree.Extend("master");
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello"));
        }

        [Test]
        public void Should_throw_if_required_override_is_missing()
        {
            var model = new { };
            RegisterTemplate("master", SyntaxTree.Block(SyntaxTree.WriteString("Hello"), SyntaxTree.Override("foo")));
            var template = SyntaxTree.Extend("master");
            Assert.Throws<VeilCompilerException>(() =>
            {
                ExecuteTemplate(template, model);
            });
        }

        [Test]
        public void Should_use_default_for_a_section_if_not_overridden()
        {
            var model = new { };
            RegisterTemplate("master", SyntaxTree.Block(SyntaxTree.WriteString("Hello "), SyntaxTree.Override("foo", SyntaxTree.WriteString("World"))));
            var template = SyntaxTree.Extend("master");
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello World"));
        }

        [Test]
        public void Should_be_able_to_nest_extends()
        {
            var model = new { };
            RegisterTemplate("one", SyntaxTree.Block(SyntaxTree.Override("start"), SyntaxTree.Override("middle"), SyntaxTree.Override("end")));
            RegisterTemplate("two", SyntaxTree.Extend("one", new Dictionary<string, SyntaxTreeNode> {
                {"start", SyntaxTree.WriteString("Hello ")},
                {"middle", SyntaxTree.WriteString("there ")},
                {"end", SyntaxTree.Override("name", SyntaxTree.WriteString("world"))}
            }));
            var template = SyntaxTree.Extend("two", new Dictionary<string, SyntaxTreeNode>
            {
                {"start", SyntaxTree.WriteString("Well hello ")},
                {"name", SyntaxTree.WriteString("Bob")}
            });
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Well hello there Bob"));
        }
    }
}