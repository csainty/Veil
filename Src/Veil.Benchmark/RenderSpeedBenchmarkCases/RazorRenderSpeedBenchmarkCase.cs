using System;
using System.Collections.Generic;
using System.Dynamic;
using RazorEngine;

namespace Veil.Benchmark.RenderSpeedBenchmarkCases
{
    public class RazorRenderSpeedBenchmarkCase : IRenderSpeedBenchmarkCase
    {
        public string Name { get { return "Razor"; } }

        public bool SupportsDictionaryModel { get { return false; } }

        public bool SupportsDynamicModel { get { return true; } }

        public RazorRenderSpeedBenchmarkCase()
        {
            var template = Templates.ReadTemplate("Template.cshtml");
            Razor.Compile<ViewModel>(template, "typed");
            Razor.Compile(template, typeof(ViewModel), "untyped");
            Razor.Compile<ExpandoObject>(template, "dynamic");
        }

        public string RenderTypedModel(ViewModel model)
        {
            return Razor.Run<ViewModel>("typed", model);
        }

        public string RenderUntypedModel(object model)
        {
            return Razor.Run("untyped", model);
        }

        public string RenderDynamicModel(dynamic model)
        {
            return Razor.Run<dynamic>("dynamic", model);
        }

        public string RenderDictionaryModel(IDictionary<string, object> model)
        {
            throw new NotSupportedException();
        }
    }
}