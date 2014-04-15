using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class WriteModelExpressionTests : CompilerTestBase
    {
        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_output_model_property<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Expression(model.GetType(), "Data"));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_output_model_field<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Expression(model.GetType(), "DataField"));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_output_model_from_sub_model<T>(T model, string expectedResult)
        {
            var template = SyntaxTreeNode.Block(SyntaxTreeNode.Expression(model.GetType(), "Sub.SubData"));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        public object[] TestCases()
        {
            return new object[] {
                new object[] { new Model<string> { DataField = "World" }, "World" },
                new object[] { new Model<int> { DataField = 123 }, "123" },
                new object[] { new Model<double> { DataField = 1.54 }, "1.54" },
                new object[] { new Model<float> { DataField = 1.1F }, "1.1" },
                new object[] { new Model<long> { DataField = 1234L }, "1234" },
                new object[] { new Model<uint> { DataField = 12U }, "12" },
                new object[] { new Model<ulong> { DataField = 12345UL }, "12345" }
            };
        }

        private class Model<T>
        {
            public T Data { get { return DataField; } }

            public T DataField;

            public SubModel<T> Sub { get { return new SubModel<T> { SubData = DataField }; } }
        }

        private class SubModel<T>
        {
            public T SubData { get; set; }
        }
    }
}