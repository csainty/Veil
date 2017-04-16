using System.IO;
using FakeItEasy;
using Veil.Parser;
using Xunit;

namespace Veil.Compiler
{
    
    public class FlushTests : CompilerTestBase
    {
        [Fact]
        public void Should_flush_textwriter()
        {
            var template = SyntaxTree.Block(
                SyntaxTree.WriteString("Start"),
                SyntaxTree.Flush(),
                SyntaxTree.WriteString("End")
            );
            var compiledTemplate = new VeilTemplateCompiler<object>(this.GetTemplateByName).Compile(template);

            var writer = A.Fake<TextWriter>();
            compiledTemplate(writer, new object());
            A.CallTo(() => writer.Write("Start")).MustHaveHappened()
                .Then(A.CallTo(() => writer.Flush()).MustHaveHappened())
                .Then(A.CallTo(() => writer.Write("End")).MustHaveHappened());
        }
    }
}