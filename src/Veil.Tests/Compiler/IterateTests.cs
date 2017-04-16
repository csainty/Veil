using System.Collections.Generic;
using Veil.Parser;
using Xunit;

namespace Veil.Compiler
{
    
    public class IterateTests : CompilerTestBase
    {
        [Fact]
        public void Should_be_able_to_iterate_a_list()
        {
            var model = new { Items = new List<string> { "1", "2" } };
            var template = SyntaxTree.Block(SyntaxTree.Iterate(
                SyntaxTreeExpression.Property(model.GetType(), "Items"),
                SyntaxTree.Block(
                    SyntaxTree.WriteString("Item")
                ),
                SyntaxTree.Block(
                    SyntaxTree.WriteString("Empty")
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.Equal("ItemItem", result);
        }

        [Fact]
        public void Should_be_able_to_iterate_an_array()
        {
            var model = new { Items = new[] { "1", "2" } };
            var template = SyntaxTree.Block(SyntaxTree.Iterate(
                SyntaxTreeExpression.Property(model.GetType(), "Items"),
                SyntaxTree.Block(
                    SyntaxTree.WriteString("Item")
                ),
                SyntaxTree.Block(
                    SyntaxTree.WriteString("Empty")
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.Equal("ItemItem", result);
        }

        [Fact]
        public void Should_use_items_from_collection_as_scope()
        {
            var model = new { Items = new[] { new ItemModel { Name = "John" }, new ItemModel { Name = "Kim" } } };
            var template = SyntaxTree.Block(SyntaxTree.Iterate(
                SyntaxTreeExpression.Property(model.GetType(), "Items"),
                SyntaxTree.Block(
                    SyntaxTree.WriteExpression(SyntaxTreeExpression.Property(typeof(ItemModel), "Name"))
                )
            ));
            var result = ExecuteTemplate(template, model);
            Assert.Equal("JohnKim", result);
        }

        [Fact]
        public void Should_be_able_to_output_value_types_from_collections()
        {
            var model = new { Items = new[] { "1", "2", "3", "4" } };
            var template = SyntaxTree.Block(SyntaxTree.Iterate(
                SyntaxTreeExpression.Property(model.GetType(), "Items"),
                SyntaxTree.Block(
                    SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(string)))
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.Equal("1234", result);
        }

        [Fact]
        public void Should_be_able_to_iterate_items_from_an_untyped_model()
        {
            var model = new Dictionary<string, object>();
            model.Add("Items", new string[] { "1", "2" });
            var template = SyntaxTree.Block(SyntaxTree.Iterate(
                SyntaxTreeExpression.LateBound("Items"),
                SyntaxTree.Block(
                    SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(object)))
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.Equal("12", result);
        }

        [Fact]
        public void Should_render_empty_block_no_items_in_collection()
        {
            var model = new { Items = new List<string> { } };
            var template = SyntaxTree.Block(SyntaxTree.Iterate(
                SyntaxTreeExpression.Property(model.GetType(), "Items"),
                SyntaxTree.Block(
                    SyntaxTree.WriteString("Item")
                ),
                SyntaxTree.Block(
                    SyntaxTree.WriteString("Empty")
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.Equal("Empty", result);
        }

        [Fact]
        public void Should_render_empty_block_no_items_in_array()
        {
            var model = new { Items = new string[0] };
            var template = SyntaxTree.Block(SyntaxTree.Iterate(
                SyntaxTreeExpression.Property(model.GetType(), "Items"),
                SyntaxTree.Block(
                    SyntaxTree.WriteString("Item")
                ),
                SyntaxTree.Block(
                    SyntaxTree.WriteString("Empty")
                )
            ));

            var result = ExecuteTemplate(template, model);
            Assert.Equal("Empty", result);
        }

        private class ItemModel
        {
            public string Name { get; set; }
        }
    }
}