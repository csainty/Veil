using System;
using System.Collections.Generic;
using System.IO;
using Veil.Benchmark.Hosts;

namespace Veil.Benchmark.RenderSpeedBenchmarkCases
{
    public class VeilHandlebarsRenderSpeedBenchmarkCase : IRenderSpeedBenchmarkCase
    {
        public string Name { get { return "Veil.Handlebars"; } }

        public bool SupportsDictionaryModel { get { return false; } }

        public bool SupportsDynamicModel { get { return false; } }

        private readonly Action<TextWriter, ViewModel> compiledTypedTemplate;
        private readonly Action<TextWriter, object> compiledUntypedTemplate;

        public VeilHandlebarsRenderSpeedBenchmarkCase()
        {
            var templateContents = Templates.ReadTemplate("Template.haml");
            var context = new VeilContext();
            context.RegisterTemplate("Roles", Templates.ReadTemplate("Roles.haml"));
            var engine = new VeilEngine(context);

            compiledTypedTemplate = engine.Compile<ViewModel>("haml", new StringReader(templateContents));
            compiledUntypedTemplate = engine.CompileNonGeneric("haml", new StringReader(templateContents), typeof(ViewModel));
        }

        public string RenderTypedModel(ViewModel model)
        {
            using (var writer = new StringWriter())
            {
                compiledTypedTemplate(writer, model);
                return writer.ToString();
            }
        }

        public string RenderDynamicModel(dynamic model)
        {
            throw new NotSupportedException();
        }

        public string RenderDictionaryModel(IDictionary<string, object> model)
        {
            throw new NotSupportedException();
        }

        public string RenderUntypedModel(object model)
        {
            using (var writer = new StringWriter())
            {
                compiledUntypedTemplate(writer, model);
                return writer.ToString();
            }
        }
    }
}