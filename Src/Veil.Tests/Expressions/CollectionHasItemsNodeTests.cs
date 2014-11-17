using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.Expressions
{
    [TestFixture]
    internal class CollectionHasItemsNodeTests
    {
        [TestCaseSource("InvalidCases")]
        public void Should_throw_when_collection_expression_not_suitable<T>(T model)
        {
            Assert.Throws<VeilParserException>(() =>
            {
                SyntaxTreeExpression.HasItems(SyntaxTreeExpression.Property(model.GetType(), "Items"));
            });
        }

        [TestCaseSource("ValidCases")]
        public void Should_not_throw_when_collection_expression_is_a_suitable_type<T>(T model)
        {
            Assert.DoesNotThrow(() =>
            {
                SyntaxTreeExpression.HasItems(SyntaxTreeExpression.Property(model.GetType(), "Items"));
            });
        }

        public object[] ValidCases()
        {
            return new object[] {
                new object[] { new { Items = new ArrayList() } },
                new object[] { new { Items = new List<string>() } },
                new object[] { new { Items = new Dictionary<string, string>() } },
                new object[] { new { Items = new [] { "" } } },
                new object[] { new { Items = new object[0] } },
                new object[] { new { Items = (ICollection<object>)new object[0] } }
            };
        }

        public object[] InvalidCases()
        {
            return new object[] {
                new object[] { new { Items = "" } },
                new object[] { new { Items = 1 } },
                new object[] { new { Items = false } },
                new object[] { new { Items = DateTime.Now } },
                new object[] { new { Items = new object() } }
            };
        }
    }
}