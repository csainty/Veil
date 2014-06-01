using System;
using System.Collections.Generic;
using System.IO;

namespace Veil.Benchmark.RenderSpeedBenchmarkCases
{
    public class RazorRenderSpeedBenchmarkCase : IRenderSpeedBenchmarkCase
    {
        public string Name { get { return "Razor"; } }

        public bool SupportsDictionaryModel { get { return false; } }

        public bool SupportsDynamicModel { get { return true; } }

        public bool SupportsUntypedModel { get { return false; } }

        private readonly Action<TextWriter, ViewModel> typedTemplate;
        private readonly Action<TextWriter, dynamic> dynamicTemplate;

        public RazorRenderSpeedBenchmarkCase()
        {
            var template = Templates.ReadTemplate("Template.cshtml");
            this.typedTemplate = Razor.Compile<ViewModel>(template);
            this.dynamicTemplate = Razor.Compile<dynamic>(template);
        }

        public string RenderTypedModel(ViewModel model)
        {
            using (var writer = new StringWriter())
            {
                this.typedTemplate(writer, model);
                return writer.ToString();
            }
        }

        public string RenderUntypedModel(object model)
        {
            throw new NotSupportedException();
        }

        public string RenderDynamicModel(dynamic model)
        {
            using (var writer = new StringWriter())
            {
                this.dynamicTemplate(writer, model);
                return writer.ToString();
            }
        }

        public string RenderDictionaryModel(IDictionary<string, object> model)
        {
            throw new NotSupportedException();
        }
    }
}