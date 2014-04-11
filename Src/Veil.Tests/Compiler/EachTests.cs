using System.Collections.Generic;
using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class EachTests : CompilerTestBase
    {
        [Test]
        public void Should_be_able_to_iterate_a_list()
        {
            var model = new { Items = new List<string> { "1", "2" } };
            var template = CreateTemplate(EachNode.Create(
                ModelPropertyExpressionNode.Create(model.GetType(), "Items"),
                BlockNode.Create(new[] {
                    WriteLiteralNode.String("Item")
                })
            ));

            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("ItemItem"));
        }

        [Test]
        public void Should_be_able_to_iterate_an_array()
        {
            var model = new { Items = new[] { "1", "2" } };
            var template = CreateTemplate(EachNode.Create(
                ModelPropertyExpressionNode.Create(model.GetType(), "Items"),
                BlockNode.Create(new[] {
                    WriteLiteralNode.String("Item")
                })
            ));

            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("ItemItem"));
        }
    }
}