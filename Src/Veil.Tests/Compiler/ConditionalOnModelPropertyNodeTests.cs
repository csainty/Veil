using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class ConditionalOnModelPropertyNodeTests : CompilerTestBase
    {
        [TestCaseSource("TruthyFalseyCases")]
        public void Should_render_correct_block_based_on_model_property<T>(T model, string expectedResult)
        {
            var template = CreateTemplate(ConditionalOnModelPropertyNode.Create(
                model.GetType(),
                "Condition",
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
                ConditionalOnModelPropertyNode.Create(
                    model.GetType(), "Condition1",
                    CreateBlock(
                        CreateStringLiteral("True1 "),
                        ConditionalOnModelPropertyNode.Create(
                            model.GetType(), "Condition2",
                            CreateBlock(CreateStringLiteral("True2 ")),
                            CreateBlock(CreateStringLiteral("False2 "))
                        )
                    ),
                    CreateBlock(
                        CreateStringLiteral("False1 "),
                        ConditionalOnModelPropertyNode.Create(
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
            var template = CreateTemplate(ConditionalOnModelPropertyNode.Create(model.GetType(), "X", trueNode, CreateBlock()));
            Assert.Throws<VeilCompilerException>(() =>
            {
                this.ExecuteTemplate(template, model);
            });
        }

        [TestCaseSource("EmptyBlockCases")]
        public void Should_handle_empty_false_block(BlockNode falseBlock)
        {
            var model = new { X = true };
            var template = CreateTemplate(ConditionalOnModelPropertyNode.Create(model.GetType(), "X", CreateBlock(CreateStringLiteral("Hello")), falseBlock));
            var result = this.ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Hello"));
        }

        public object[] TruthyFalseyCases()
        {
            return new object[] {
                new object[] { new { Condition = true }, "True" },
                new object[] { new { Condition = false }, "False" },
                new object[] { new { Condition = 1 }, "True" },
                new object[] { new { Condition = 0 }, "False" },
                new object[] { new { Condition = new object() }, "True" },
                new object[] { new { Condition = (object)null }, "False" }
            };
        }

        public object[] EmptyBlockCases()
        {
            return new object[] {
                new object[] { null },
                new object[] { CreateBlock() }
            };
        }
    }
}