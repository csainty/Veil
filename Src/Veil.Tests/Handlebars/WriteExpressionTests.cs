using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class WriteExpressionTests : ParserTestBase<HandlebarsParser>
    {
        [Theory]
        [MemberData("PropertyNameTestSource")]
        public void Should_parse_model_property_names(string template, SyntaxTreeNode[] expectedTemplate)
        {
            var syntaxTree = Parse(template, typeof(TestModel));
            AssertSyntaxTree(syntaxTree, expectedTemplate);
        }

        public static object[] PropertyNameTestSource()
        {
            return new object[] {
                new object[] {"{{Name}}", new SyntaxTreeNode[] { SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), true) } },
                new object[] {"{{ Name }}", new SyntaxTreeNode[] { SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), true) } },
                new object[] {"Hello {{Name}}", new SyntaxTreeNode[] { SyntaxTree.WriteString("Hello "), SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), true) } },
                new object[] {"Hello {{Name}}, {{ Greeting }}", new SyntaxTreeNode[] { SyntaxTree.WriteString("Hello "), SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), true), SyntaxTree.WriteString(", "), SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Greeting"), true) } },
                new object[] {"{{{ Name }}}", new SyntaxTreeNode[] { SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(TestModel), "Name"), false) }}
            };
        }

        [Fact]
        public void Should_throw_if_property_not_part_OF_model()
        {
            var model = new { Name = "" };
            Assert.Throws<VeilParserException>(() =>
            {
                Parse("Hello {{ foo }} !", model.GetType());
            });
        }

        [Theory]
        [InlineData("{Name}")]
        [InlineData("Hello {Name}")]
        [InlineData("Hello { this string { contains { opening identifiers")]
        [InlineData("{{ name }")]
        [InlineData("Hello {{ name }")]
        [InlineData("Hello {{ name } World")]
        [InlineData("{ name }}")]
        [InlineData("Hello { name }}")]
        [InlineData("Hello { name }} World")]
        [InlineData("{{{ Name }}")]
        [InlineData("{{{{ Name }}}}")]
        [InlineData("Hello {{{{ Name }}}}")]
        [InlineData("Hello {{{{ Name }}}} World")]
        public void Should_handle_incomplete_identifier_marker(string testString)
        {
            var syntaxTree = Parse(testString, typeof(object));
            AssertSyntaxTree(syntaxTree, SyntaxTree.WriteString(testString));
        }

        private class TestModel
        {
            public string Name { get; set; }

            public string Greeting { get; set; }
        }
    }
}