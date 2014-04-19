using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class ConditionalOnExpressionTests : CompilerTestBase
    {
        [TestCaseSource("TruthyFalseyCases")]
        public void Should_render_correct_block_based_on_model_property<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Conditional(
                SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Condition"),
                SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("True")),
                SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("False"))));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("TruthyFalseyCases")]
        public void Should_render_correct_block_based_on_model_field<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Conditional(
                SyntaxTreeNode.ExpressionNode.ModelField(model.GetType(), "ConditionField"),
                SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("True")),
                SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("False"))));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(true, true, "Start True1 True2 End")]
        [TestCase(true, false, "Start True1 False2 End")]
        [TestCase(false, true, "Start False1 True2 End")]
        [TestCase(false, false, "Start False1 False2 End")]
        public void Should_be_able_to_nest_conditionals(bool c1, bool c2, string expectedResult)
        {
            var model = new { Condition1 = c1, Condition2 = c2 };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.StringLiteral("Start "),
                SyntaxTreeNode.Conditional(
                    SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Condition1"),
                    SyntaxTreeNode.Block(
                        SyntaxTreeNode.StringLiteral("True1 "),
                        SyntaxTreeNode.Conditional(
                            SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Condition2"),
                            SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("True2 ")),
                            SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("False2 "))
                        )
                    ),
                    SyntaxTreeNode.Block(
                        SyntaxTreeNode.StringLiteral("False1 "),
                        SyntaxTreeNode.Conditional(
                            SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Condition2"),
                            SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("True2 ")),
                            SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("False2 "))
                        )
                    )
                ),
                SyntaxTreeNode.StringLiteral("End"));
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("EmptyBlockCases")]
        public void Should_throw_with_empty_true_block(SyntaxTreeNode.BlockNode trueNode)
        {
            var model = new { X = true };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.Conditional(
                SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "X"),
                trueNode,
                SyntaxTreeNode.Block()
            ));

            Assert.Throws<VeilCompilerException>(() =>
            {
                this.ExecuteTemplate(template, model);
            });
        }

        [TestCaseSource("EmptyBlockCases")]
        public void Should_handle_empty_false_block(SyntaxTreeNode.BlockNode falseBlock)
        {
            var model = new { X = true };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.Conditional(
                    SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "X"),
                    SyntaxTreeNode.Block(SyntaxTreeNode.StringLiteral("Hello")),
                    falseBlock)
                );
            var result = this.ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello"));
        }

        [Test]
        public void Should_handle_conditional_on_root_scope()
        {
            var model = new { RootConditional = true, Values = new[] { 1, 2, 3 } };
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.Each(
                    SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "Values"),
                    SyntaxTreeNode.Block(
                        SyntaxTreeNode.Conditional(
                            SyntaxTreeNode.ExpressionNode.ModelProperty(model.GetType(), "RootConditional", SyntaxTreeNode.ExpressionScope.RootModel),
                            SyntaxTreeNode.Block(SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.Self(typeof(int))))
                        )
                    )
                )
            );
            var result = this.ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("123"));
        }

        public object[] TruthyFalseyCases()
        {
            return new object[] {
                new object[] { new Model<bool>{ ConditionField = true }, "True" },
                new object[] { new Model<bool>{ ConditionField = false }, "False" },
                new object[] { new Model<int>{ ConditionField = 1 }, "True" },
                new object[] { new Model<int>{ ConditionField = 0 }, "False" },
                new object[] { new Model<object>{ ConditionField = new object() }, "True" },
                new object[] { new Model<object>{ ConditionField = (object)null }, "False" }
            };
        }

        public object[] EmptyBlockCases()
        {
            return new object[] {
                new object[] { null },
                new object[] { SyntaxTreeNode.Block() }
            };
        }

        private class Model<T>
        {
            public T ConditionField;

            public T Condition { get { return this.ConditionField; } }
        }
    }
}