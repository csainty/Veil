using System;
using System.IO;
using NUnit.Framework;

namespace Veil.HelperTests
{
    [TestFixture]
    public class HtmlEncodeTests
    {
        [TestCaseSource("TestCases")]
        public void Should_encode_html(string input, string expectedOutput)
        {
            var writer = new StringWriter();
            Veil.Helpers.HtmlEncode(writer, input);
            Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
        }

        [TestCaseSource("TestCases")]
        public void Should_encode_long_html(string input, string expectedOutput)
        {
            var longInput = input + new String('x', 100);
            var longExpectedOutput = expectedOutput + new String('x', 100);

            var writer = new StringWriter();
            Veil.Helpers.HtmlEncode(writer, longInput);
            Assert.That(writer.ToString(), Is.EqualTo(longExpectedOutput));
        }

        public object[] TestCases()
        {
            return new object[] {
                new object[] { "", "" },
                new object[] { "Hello", "Hello" },
                new object[] { "Hello&Goodbye", "Hello&amp;Goodbye" },
                new object[] { "Hello<Goodbye", "Hello&lt;Goodbye" },
                new object[] { "Hello>Goodbye", "Hello&gt;Goodbye" },
                new object[] { "Hello\"Goodbye", "Hello&quot;Goodbye" },
                new object[] { "Hello'Goodbye", "Hello&#39;Goodbye" },
                new object[] { "&Hello", "&amp;Hello" },
                new object[] { "Hello&", "Hello&amp;" },
            };
        }
    }
}