using System;
using System.IO;

namespace Veil.Benchmark.GCBenchmarkCases
{
    public class ChevronGCBenchmarkCase : IGCBenchmarkCase, IDisposable
    {
        public string Name { get { return "Chevron.IE.Merged"; } }

        private readonly Chevron.Handlebars engine = new Chevron.Handlebars();

        public ChevronGCBenchmarkCase()
        {
            engine.RegisterPartial("Roles", Templates.ReadTemplate("Roles.hbs"));
            engine.RegisterTemplate("default", Templates.ReadTemplate("Template.hbs"));
        }

        public void Render(TextWriter writer, ViewModel model)
        {
            writer.Write(engine.Transform("default", model));
        }

        public void Dispose()
        {
            engine.Dispose();
        }
    }
}