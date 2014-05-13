using System.IO;
using Veil.Benchmark.Hosts;

namespace Veil.Benchmark.GCBenchmarkCases
{
    public class SuperSimpleGCBenchmarkCase : IGCBenchmarkCase
    {
        public string Name { get { return "SuperSimple"; } }

        private readonly string templateContents = Templates.ReadTemplate("Template.sshtml");
        private readonly SuperSimpleHost host = new SuperSimpleHost();
        private readonly SuperSimpleViewEngine.SuperSimpleViewEngine engine = new SuperSimpleViewEngine.SuperSimpleViewEngine();

        public SuperSimpleGCBenchmarkCase()
        {
            host.RegisterTemplate("Master", Templates.ReadTemplate("Master.sshtml"));
            host.RegisterTemplate("Roles", Templates.ReadTemplate("Roles.sshtml"));
        }

        public void Render(TextWriter writer, ViewModel model)
        {
            writer.Write(engine.Render(templateContents, model, host));
        }
    }
}