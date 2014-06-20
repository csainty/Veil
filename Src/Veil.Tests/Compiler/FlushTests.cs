using System.IO;
using FakeItEasy;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    [TestFixture]
    internal class FlushTests : CompilerTestBase
    {
        [Test]
        public void Should_flush_textwriter()
        {
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Start"),
                SyntaxTree.Flush(),
                SyntaxTree.WriteString("End")
            );
            var compiledTemplate = new VeilTemplateCompiler<object>(this.GetTemplateByName).Compile(template);

            using (var scope = Fake.CreateScope())
            {
                var writer = A.Fake<TextWriter>();
                compiledTemplate(writer, new object());
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => writer.Write("Start")).MustHaveHappened();
                    A.CallTo(() => writer.Flush()).MustHaveHappened();
                    A.CallTo(() => writer.Write("End")).MustHaveHappened();
                }
            }
        }
    }
}