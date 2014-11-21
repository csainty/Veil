using System;
using System.Collections.Generic;
using System.IO;
using Veil.Benchmark.Hosts;

namespace Veil.Benchmark.RenderSpeedBenchmarkCases
{
    public class VeilHandlebarsRenderSpeedBenchmarkCase : IRenderSpeedBenchmarkCase
    {
        public string Name { get { return "Veil.Handlebars"; } }

        public bool SupportsDictionaryModel { get { return true; } }

        public bool SupportsDynamicModel { get { return true; } }

        public bool SupportsUntypedModel { get { return true; } }

        private readonly Action<TextWriter, ViewModel> compiledTypedTemplate;
        private readonly Action<TextWriter, object> compiledUntypedTemplate;
        private readonly Action<TextWriter, dynamic> compiledDynamicTemplate;
        private readonly Action<TextWriter, IDictionary<string, object>> compiledDictionaryTemplate;

        public VeilHandlebarsRenderSpeedBenchmarkCase()
        {
            var templateContents = Templates.ReadTemplate("Template.hbs");
            var context = new VeilContext();
            context.RegisterTemplate("Roles", Templates.ReadTemplate("Roles.hbs"));
            var engine = new VeilEngine(context);

            compiledTypedTemplate = engine.Compile<ViewModel>("hbs", new StringReader(templateContents));
            compiledUntypedTemplate = engine.CompileNonGeneric("hbs", new StringReader(templateContents), typeof(ViewModel));
            compiledDynamicTemplate = engine.Compile<dynamic>("hbs", new StringReader(templateContents));
            compiledDictionaryTemplate = engine.Compile<IDictionary<string, object>>("hbs", new StringReader(templateContents));
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
            using (var writer = new StringWriter())
            {
                compiledDynamicTemplate(writer, model);
                return writer.ToString();
            }
        }

        public string RenderDictionaryModel(IDictionary<string, object> model)
        {
            using (var writer = new StringWriter())
            {
                compiledDictionaryTemplate(writer, model);
                return writer.ToString();
            }
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