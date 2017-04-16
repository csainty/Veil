using System;
using System.Collections;
using System.Collections.Generic;
using Veil.Parser;
using Xunit;

namespace Veil.Expressions
{
    
    public class IterateNodeTests
    {
        [Theory]
        [MemberData("InvalidCases")]
        public void Should_throw_when_collection_expression_not_suitable<T>(T model)
        {
            Assert.Throws<VeilParserException>(() =>
            {
                SyntaxTree.Iterate(SyntaxTreeExpression.Property(model.GetType(), "Items"), SyntaxTree.Block());
            });
        }

        [Theory]
        [MemberData("ValidCases")]
        public void Should_not_throw_when_collection_expression_is_a_suitable_type<T>(T model)
        {
            SyntaxTree.Iterate(SyntaxTreeExpression.Property(model.GetType(), "Items"), SyntaxTree.Block());
        }

        public static object[] ValidCases()
        {
            return new object[] {
                new object[] { new { Items = new List<string>() } },
                new object[] { new { Items = new Dictionary<string, string>() } },
                new object[] { new { Items = new [] { "" } } },
                new object[] { new { Items = "" } },
                new object[] { new { Items = new object() }}
            };
        }

        public static object[] InvalidCases()
        {
            return new object[] {
                new object[] { new { Items = new ArrayList() } },
                new object[] { new { Items = 1 } },
                new object[] { new { Items = false } },
                new object[] { new { Items = DateTime.Now } }
            };
        }
    }
}