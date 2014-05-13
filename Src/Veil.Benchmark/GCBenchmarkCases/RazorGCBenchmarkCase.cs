using System.IO;
using RazorEngine;

namespace Veil.Benchmark.GCBenchmarkCases
{
    public class RazorGCBenchmarkCase : IGCBenchmarkCase
    {
        public string Name { get { return "Razor"; } }

        public RazorGCBenchmarkCase()
        {
            var template = Templates.ReadTemplate("Template.cshtml");
            Razor.Compile<ViewModel>(template, "typed");
        }

        public void Render(TextWriter writer, ViewModel model)
        {
            writer.Write(Razor.Run<ViewModel>("typed", model));
        }
    }
}