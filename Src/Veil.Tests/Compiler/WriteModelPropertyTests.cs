using NUnit.Framework;

namespace Veil.Compiler
{
    [TestFixture]
    internal class WriteModelPropertyTests : CompilerTestBase
    {
        [SetCulture("en-US")]
        [TestCaseSource("TestCases")]
        public void Should_be_able_to_output_model_property<T>(T model, string expectedResult)
        {
            var template = CreateTemplate(WriteModelPropertyNode.Create(model.GetType(), "Data"));
            var result = ExecuteTemplate(template, model);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        public object[] TestCases()
        {
            return new object[] {
                new object[] { new Model<string> { Data = "World" }, "World" },
                new object[] { new Model<int> { Data = 123 }, "123" },
                new object[] { new Model<double> { Data = 1.54 }, "1.54" },
                new object[] { new Model<float> { Data = 1.1F }, "1.1" },
                new object[] { new Model<long> { Data = 1234L }, "1234" },
                new object[] { new Model<uint> { Data = 12U }, "12" },
                new object[] { new Model<ulong> { Data = 12345UL }, "12345" }
            };
        }

        private class Model<T>
        {
            public T Data { get; set; }
        }
    }
}