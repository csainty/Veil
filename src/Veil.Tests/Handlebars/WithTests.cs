﻿using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class WithTests : ParserTestBase<HandlebarsParser>
    {
        [Fact]
        public void Should_parse_a_with_binding()
        {
            var model = new { User = new { Name = "" }, Dept = new { Name = "" } };
            var template = Parse("Hello {{#with User}}{{Name}}{{/with}} from {{#with Dept}}{{Name}}{{/with}}", model.GetType());
            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.ScopeNode(
                    SyntaxTreeExpression.Property(model.GetType(), "User"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.User.GetType(), "Name"), true)
                    )
                ),
                SyntaxTree.WriteString(" from "),
                SyntaxTree.ScopeNode(
                    SyntaxTreeExpression.Property(model.GetType(), "Dept"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.Dept.GetType(), "Name"), true)
                    )
                )
            );
        }

        [Fact]
        public void Should_support_this_inside_with_block()
        {
            var model = new { User = "" };
            var template = Parse("Hello {{#with User}}{{this}}{{/with}}", model.GetType());
            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.ScopeNode(
                    SyntaxTreeExpression.Property(model.GetType(), "User"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(model.User.GetType()), true)
                    )
                )
            );
        }

        [Fact]
        public void Should_support_parent_inside_with_block()
        {
            var model = new { User = "", Name = "" };
            var template = Parse("Hello {{#with User}}{{../Name}}{{this}}{{/with}}", model.GetType());
            AssertSyntaxTree(
                template,
                SyntaxTree.WriteString("Hello "),
                SyntaxTree.ScopeNode(
                    SyntaxTreeExpression.Property(model.GetType(), "User"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(model.GetType(), "Name", ExpressionScope.ModelOfParentScope), true),
                        SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(model.User.GetType()), true)
                    )
                )
            );
        }

        [Theory]
        [InlineData("Hello {{#with Sub}} There")]
        [InlineData("Hello {{/with}} There")]
        [InlineData("Hello {{#with Sub}} There{{/with}}{{/with}}")]
        public void Should_throw_if_block_not_open_and_closed_consistently(string template)
        {
            var model = new { Sub = new { } };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse(template, model.GetType());
            });
        }
    }
}