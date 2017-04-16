using System.Collections.Generic;
using DeepEqual.Syntax;
using Xunit;
using Veil.Parser;

namespace Veil.Handlebars
{
    
    public class ExtendTests : ParserTestBase<HandlebarsParser>
    {
        [Fact]
        public void Should_parse_extend_pages()
        {
            var result = Parse("{{< Master}}Hello World");
            result.ShouldDeepEqual(SyntaxTree.Extend("Master", new Dictionary<string, SyntaxTreeNode>
            {
                {"body", SyntaxTree.Block(SyntaxTree.WriteString("Hello World"))}
            }));
        }

        [Fact]
        public void Should_parse_body_override()
        {
            var result = Parse("<p>{{body}}</p>");
            AssertSyntaxTree(
                result,
                SyntaxTree.WriteString("<p>"),
                SyntaxTree.Override("body", false),
                SyntaxTree.WriteString("</p>")
            );
        }

        [Fact]
        public void Should_throw_when_extend_is_not_first_node()
        {
            Assert.Throws<VeilParserException>(() =>
            {
                Parse("Hello {{< Master }}");
            });
        }
    }
}