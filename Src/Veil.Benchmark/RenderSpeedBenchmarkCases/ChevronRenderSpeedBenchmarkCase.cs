using System;
using System.Collections.Generic;

namespace Veil.Benchmark.RenderSpeedBenchmarkCases
{
    public class ChevronRenderSpeedBenchmarkCase : IRenderSpeedBenchmarkCase, IDisposable
    {
        public string Name { get { return "Chevron.IE.Merged"; } }

        public bool SupportsDictionaryModel { get { return true; } }

        public bool SupportsDynamicModel { get { return true; } }

        public bool SupportsUntypedModel { get { return true; } }

        private readonly Chevron.Handlebars engine = new Chevron.Handlebars();

        public ChevronRenderSpeedBenchmarkCase()
        {
            engine.RegisterPartial("Roles", Templates.ReadTemplate("Roles.hbs"));
            engine.RegisterTemplate("default", Templates.ReadTemplate("Template.hbs"));
        }

        public string RenderTypedModel(ViewModel model)
        {
            return engine.Transform("default", model);
        }

        public string RenderUntypedModel(object model)
        {
            return engine.Transform("default", model);
        }

        public string RenderDynamicModel(dynamic model)
        {
            return engine.Transform("default", model);
        }

        public string RenderDictionaryModel(IDictionary<string, object> model)
        {
            return engine.Transform("default", model);
        }

        public void Dispose()
        {
            engine.Dispose();
        }
    }
}