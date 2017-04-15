using System.Collections.Generic;
using System.Reflection;
using Veil.Parser;
using Veil.Parser.Nodes;
using Xunit;

namespace Veil.Compiler
{
    
    public class ConditionalTests : CompilerTestBase
    {
        [Theory]
        [MemberData("TruthyFalseyCases")]
        public void Should_render_correct_block_based_on_model_property<T>(T model, string expectedResult)
        {
            var template = SyntaxTree.Block(SyntaxTree.Conditional(
                SyntaxTreeExpression.Property(model.GetType(), "Condition"),
                SyntaxTree.Block(SyntaxTree.WriteString("True")),
                SyntaxTree.Block(SyntaxTree.WriteString("False"))));
            var result = ExecuteTemplate(template, model);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [MemberData("TruthyFalseyCases")]
        public void Should_render_correct_block_based_on_model_field<T>(T model, string expectedResult)
        {
            var template = SyntaxTree.Block(SyntaxTree.Conditional(
                SyntaxTreeExpression.Field(model.GetType(), "ConditionField"),
                SyntaxTree.Block(SyntaxTree.WriteString("True")),
                SyntaxTree.Block(SyntaxTree.WriteString("False"))));
            var result = ExecuteTemplate(template, model);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(true, true, "Start True1 True2 End")]
        [InlineData(true, false, "Start True1 False2 End")]
        [InlineData(false, true, "Start False1 True2 End")]
        [InlineData(false, false, "Start False1 False2 End")]
        public void Should_be_able_to_nest_conditionals(bool c1, bool c2, string expectedResult)
        {
            var model = new { Condition1 = c1, Condition2 = c2 };
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Start "),
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.Property(model.GetType(), "Condition1"),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("True1 "),
                        SyntaxTree.Conditional(
                            SyntaxTreeExpression.Property(model.GetType(), "Condition2"),
                            SyntaxTree.Block(SyntaxTree.WriteString("True2 ")),
                            SyntaxTree.Block(SyntaxTree.WriteString("False2 "))
                        )
                    ),
                    SyntaxTree.Block(
                        SyntaxTree.WriteString("False1 "),
                        SyntaxTree.Conditional(
                            SyntaxTreeExpression.Property(model.GetType(), "Condition2"),
                            SyntaxTree.Block(SyntaxTree.WriteString("True2 ")),
                            SyntaxTree.Block(SyntaxTree.WriteString("False2 "))
                        )
                    )
                ),
                SyntaxTree.WriteString("End"));
            var result = ExecuteTemplate(template, model);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [MemberData("EmptyBlockCases")]
        public void Should_throw_with_empty_blocks(BlockNode emptyBlock)
        {
            var model = new { X = true };
            var template = SyntaxTree.Block(
                SyntaxTree.Conditional(
                SyntaxTreeExpression.Property(model.GetType(), "X"),
                emptyBlock,
                emptyBlock
            ));

            Assert.Throws<VeilCompilerException>(() =>
            {
                this.ExecuteTemplate(template, model);
            });
        }

        [Theory]
        [MemberData("EmptyBlockCases")]
        public void Should_handle_empty_false_block(BlockNode falseBlock)
        {
            var model = new { X = true };
            var template = SyntaxTree.Block(
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.Property(model.GetType(), "X"),
                    SyntaxTree.Block(SyntaxTree.WriteString("Hello")),
                    falseBlock)
                );
            var result = this.ExecuteTemplate(template, model);
            Assert.Equal("Hello", result);
        }

        [Theory]
        [MemberData("EmptyBlockCases")]
        public void Should_handle_empty_true_block(BlockNode trueBlock)
        {
            var model = new { X = false };
            var template = SyntaxTree.Block(
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.Property(model.GetType(), "X"),
                    trueBlock,
                    SyntaxTree.Block(SyntaxTree.WriteString("Hello")))
                );
            var result = this.ExecuteTemplate(template, model);
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void Should_handle_conditional_on_root_scope()
        {
            var model = new { RootConditional = true, Values = new[] { 1, 2, 3 } };
            var template = SyntaxTree.Block(
                SyntaxTree.Iterate(
                    SyntaxTreeExpression.Property(model.GetType(), "Values"),
                    SyntaxTree.Block(
                        SyntaxTree.Conditional(
                            SyntaxTreeExpression.Property(model.GetType(), "RootConditional", ExpressionScope.RootModel),
                            SyntaxTree.Block(SyntaxTree.WriteExpression(SyntaxTreeExpression.Self(typeof(int))))
                        )
                    )
                )
            );
            var result = this.ExecuteTemplate(template, model);
            Assert.Equal("123", result);
        }

        [Theory]
        [MemberData("HasItemsCases")]
        public void Should_handle_has_items_expression<T>(T model, string expectedResult)
        {
            var template = SyntaxTree.Block(
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.HasItems(SyntaxTreeExpression.Property(model.GetType(), "Items")),
                    SyntaxTree.Block(SyntaxTree.WriteString("HasItems")),
                    SyntaxTree.Block(SyntaxTree.WriteString("HasNoItems"))
                )
            );
            var result = this.ExecuteTemplate(template, model);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [MemberData("TruthyFalseyCases")]
        public void Should_handle_conditional_on_dictionary_item<T>(T testModel, string expectedResult)
        {
            var model = new Dictionary<string, object>();
            model.Add("Bool", testModel.GetType().GetProperty("Condition").GetValue(testModel));
            var template = SyntaxTree.Block(
                SyntaxTree.Conditional(
                    SyntaxTreeExpression.LateBound("Bool"),
                    SyntaxTree.Block(SyntaxTree.WriteString("True")),
                    SyntaxTree.Block(SyntaxTree.WriteString("False"))
                )
            );
            var result = ExecuteTemplate(template, model);
            Assert.Equal(expectedResult, result);
        }

        public static object[] TruthyFalseyCases()
        {
            return new object[] {
                new object[] { new Model<bool>{ ConditionField = true }, "True" },
                new object[] { new Model<bool>{ ConditionField = false }, "False" },
                new object[] { new Model<object>{ ConditionField = new object() }, "True" },
                new object[] { new Model<object>{ ConditionField = (object)null }, "False" }
            };
        }

        public static object[] EmptyBlockCases()
        {
            return new object[] {
                new object[] { null },
                new object[] { SyntaxTree.Block() }
            };
        }

        public static object[] HasItemsCases()
        {
            return new object[] {
                new object[] { new { Items = new[] { "Hello" } }, "HasItems" },
                new object[] { new { Items = new List<int>() }, "HasNoItems" },
                new object[] { new { Items = (ICollection<object>)new object[0] }, "HasNoItems" },
            };
        }

        private class Model<T>
        {
            public T ConditionField;

            public T Condition { get { return this.ConditionField; } }
        }
    }
}