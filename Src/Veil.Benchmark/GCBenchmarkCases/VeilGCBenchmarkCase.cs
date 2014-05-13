using System;
using System.IO;
using Veil.Benchmark.Hosts;

namespace Veil.Benchmark.GCBenchmarkCases
{
    public class VeilGCBenchmarkCase : IGCBenchmarkCase
    {
        public string Name { get { return "Veil"; } }

        private readonly Action<TextWriter, ViewModel> template;

        public VeilGCBenchmarkCase()
        {
            var templateContents = Templates.ReadTemplate("Template.sshtml");
            var context = new VeilContext();
            context.RegisterTemplate("Master", Templates.ReadTemplate("Master.sshtml"));
            context.RegisterTemplate("Roles", Templates.ReadTemplate("Roles.sshtml"));
            var engine = new VeilEngine(context);

            template = engine.Compile<ViewModel>("sshtml", new StringReader(templateContents));
        }

        public void Render(TextWriter writer, ViewModel model)
        {
            template(writer, model);
        }
    }
}