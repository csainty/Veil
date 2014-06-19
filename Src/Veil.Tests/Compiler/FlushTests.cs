using System.IO;
using FakeItEasy;
using NUnit.Framework;
using S = Veil.Parser.SyntaxTree;

namespace Veil.Compiler
{
    [TestFixture]
    internal class FlushTests : CompilerTestBase
    {
        [Test]
        public void Should_flush_textwriter()
        {
            var template = S.Block(
                S.WriteString("Start"),
                S.Flush(),
                S.WriteString("End")
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