using System.Collections.Generic;
using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class IterateTests : CompilerTestBase
    {
        [Test]
        public void Should_be_able_to_iterate_a_list()
        {
            var model = new { Items = new List<string> { "1", "2" } };
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Iterate(
                SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Items"),
                SyntaxTreeNode.Block(
                    SyntaxTreeNode.WriteString("Item")
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("ItemItem"));
        }

        [Test]
        public void Should_be_able_to_iterate_an_array()
        {
            var model = new { Items = new[] { "1", "2" } };
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Iterate(
                SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Items"),
                SyntaxTreeNode.Block(
                    SyntaxTreeNode.WriteString("Item")
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("ItemItem"));
        }

        [Test]
        public void Should_use_items_from_collection_as_scope()
        {
            var model = new { Items = new[] { new ItemModel { Name = "John" }, new ItemModel { Name = "Kim" } } };
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Iterate(
                SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Items"),
                SyntaxTreeNode.Block(
                    SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.ModelProperty(typeof(ItemModel), "Name"))
                )
            ));
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("JohnKim"));
        }

        [Test]
        public void Should_be_able_to_output_value_types_from_collections()
        {
            var model = new { Items = new[] { "1", "2", "3", "4" } };
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Iterate(
                SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Items"),
                SyntaxTreeNode.Block(
                    SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Self(typeof(string)))
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("1234"));
        }

        private class ItemModel
        {
            public string Name { get; set; }
        }
    }
}