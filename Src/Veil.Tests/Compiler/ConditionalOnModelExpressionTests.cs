using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class ConditionalOnModelExpressionTests : CompilerTestBase
    {
        [TestCaseSource("TruthyFalseyCases")]
        public void Should_render_correct_block_based_on_model_property<T>(T model, string expectedResult)
        {
            var template = CreateTemplate(ConditionalOnModelExpressionNode.Create(
                model.GetType(),
                "Condition",
                CreateBlock(CreateStringLiteral("True")),
                CreateBlock(CreateStringLiteral("False"))));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("TruthyFalseyCases")]
        public void Should_render_correct_block_based_on_model_field<T>(T model, string expectedResult)
        {
            var template = CreateTemplate(ConditionalOnModelExpressionNode.Create(
                model.GetType(),
                "ConditionField",
                CreateBlock(CreateStringLiteral("True")),
                CreateBlock(CreateStringLiteral("False"))));
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
            var template = CreateTemplate(
                CreateStringLiteral("Start "),
                ConditionalOnModelExpressionNode.Create(
                    model.GetType(), "Condition1",
                    CreateBlock(
                        CreateStringLiteral("True1 "),
                        ConditionalOnModelExpressionNode.Create(
                            model.GetType(), "Condition2",
                            CreateBlock(CreateStringLiteral("True2 ")),
                            CreateBlock(CreateStringLiteral("False2 "))
                        )
                    ),
                    CreateBlock(
                        CreateStringLiteral("False1 "),
                        ConditionalOnModelExpressionNode.Create(
                            model.GetType(), "Condition2",
                            CreateBlock(CreateStringLiteral("True2 ")),
                            CreateBlock(CreateStringLiteral("False2 "))
                        )
                    )
                ),
                CreateStringLiteral("End"));
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("EmptyBlockCases")]
        public void Should_throw_with_empty_true_block(BlockNode trueNode)
        {
            var model = new { X = true };
            var template = CreateTemplate(ConditionalOnModelExpressionNode.Create(model.GetType(), "X", trueNode, CreateBlock()));
            Assert.Throws<VeilCompilerException>(() =>
            {
                this.ExecuteTemplate(template, model);
            });
        }

        [TestCaseSource("EmptyBlockCases")]
        public void Should_handle_empty_false_block(BlockNode falseBlock)
        {
            var model = new { X = true };
            var template = CreateTemplate(ConditionalOnModelExpressionNode.Create(model.GetType(), "X", CreateBlock(CreateStringLiteral("Hello")), falseBlock));
            var result = this.ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello"));
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
                new object[] { CreateBlock() }
            };
        }

        private class Model<T>
        {
            public T ConditionField;

            public T Condition { get { return this.ConditionField; } }
        }
    }
}