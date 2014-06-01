using System;
using System.Collections.Generic;
using Veil.Benchmark.Hosts;

namespace Veil.Benchmark.RenderSpeedBenchmarkCases
{
    public class SuperSimpleRenderSpeedBenchmarkCase : IRenderSpeedBenchmarkCase
    {
        public string Name { get { return "SuperSimple"; } }

        public bool SupportsDictionaryModel { get { return false; } }

        public bool SupportsDynamicModel { get { return true; } }

        public bool SupportsUntypedModel { get { return true; } }

        private readonly string templateContents = Templates.ReadTemplate("Template.sshtml");
        private readonly SuperSimpleHost host = new SuperSimpleHost();
        private readonly SuperSimpleViewEngine.SuperSimpleViewEngine engine = new SuperSimpleViewEngine.SuperSimpleViewEngine();

        public SuperSimpleRenderSpeedBenchmarkCase()
        {
            host.RegisterTemplate("Master", Templates.ReadTemplate("Master.sshtml"));
            host.RegisterTemplate("Roles", Templates.ReadTemplate("Roles.sshtml"));
        }

        public string RenderTypedModel(ViewModel model)
        {
            return engine.Render(templateContents, model, host);
        }

        public string RenderDynamicModel(dynamic model)
        {
            return engine.Render(templateContents, model, host);
        }

        public string RenderDictionaryModel(IDictionary<string, object> model)
        {
            throw new NotSupportedException();
        }

        public string RenderUntypedModel(object model)
        {
            return engine.Render(templateContents, model, host);
        }
    }
}