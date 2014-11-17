using NUnit.Framework;
using Veil.Parser;

namespace Veil.Expressions
{
    [TestFixture]
    public class ConditionalNodeTests
    {
        [TestCaseSource("InvalidCases")]
        public void Should_throw_when_conditional_expression_not_suitable<T>(T model)
        {
            Assert.Throws<VeilParserException>(() =>
            {
                SyntaxTree.Conditional(SyntaxTreeExpression.Property(model.GetType(), "Items"), SyntaxTree.Block(), SyntaxTree.Block());
            });
        }

        [TestCaseSource("ValidCases")]
        public void Should_not_throw_when_conditional_expression_is_a_suitable_type<T>(T model)
        {
            Assert.DoesNotThrow(() =>
            {
                SyntaxTree.Conditional(SyntaxTreeExpression.Property(model.GetType(), "Items"), SyntaxTree.Block(), SyntaxTree.Block());
            });
        }

        public object[] ValidCases()
        {
            return new object[] {
                new object[] { new { Items = true } },
                new object[] { new { Items = new object() } },
                new object[] { new { Items = "" } },
                new object[] { new { Items = (object)null } },
                new object[] { new { Items = new { } } }
            };
        }

        public object[] InvalidCases()
        {
            return new object[] {
                new object[] { new { Items = 0 } },
                new object[] { new { Items = 0L } },
                new object[] { new { Items = 0L } },
                new object[] { new { Items = 0d } },
                new object[] { new { Items = 0m } },
            };
        }
    }
}