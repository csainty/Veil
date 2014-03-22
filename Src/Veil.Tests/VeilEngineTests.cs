using System;
using System.IO;
using NUnit.Framework;

namespace Veil
{
    [TestFixture]
    public class VeilEngineTests
    {
        private readonly IVeilEngine engine = new VeilEngine();

        [Test]
        public void Should_render_a_hail_template()
        {
            var view = Compile("Hello {{ Name }}. You have visited us {{ ViewCount }} times!");
            var result = Execute(view, new ViewModel { Name = "Chris", ViewCount = 10 });

            Assert.That(result, Is.EqualTo("Hello Chris. You have visited us 10 times!"));
        }

        private Action<TextWriter, ViewModel> Compile(string template)
        {
            using (var reader = new StringReader(template))
            {
                return this.engine.Compile<ViewModel>(reader);
            }
        }

        private string Execute(Action<TextWriter, ViewModel> view, ViewModel model)
        {
            using (var writer = new StringWriter())
            {
                view(writer, model);
                return writer.ToString();
            }
        }

        private class ViewModel
        {
            public string Name { get; set; }

            public int ViewCount { get; set; }
        }
    }
}