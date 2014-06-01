using System;
using System.IO;

namespace Veil.Benchmark.GCBenchmarkCases
{
    public class RazorGCBenchmarkCase : IGCBenchmarkCase
    {
        public string Name { get { return "Razor"; } }

        private readonly Action<TextWriter, ViewModel> compiledTemplate;

        public RazorGCBenchmarkCase()
        {
            var template = Templates.ReadTemplate("Template.cshtml");
            this.compiledTemplate = Razor.Compile<ViewModel>(template);
        }

        public void Render(TextWriter writer, ViewModel model)
        {
            this.compiledTemplate(writer, model);
        }
    }
}