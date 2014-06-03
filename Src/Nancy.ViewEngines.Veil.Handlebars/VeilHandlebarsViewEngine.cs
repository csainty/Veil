using System;
using System.Collections.Generic;
using System.IO;
using Veil;
using Veil.Handlebars;

namespace Nancy.ViewEngines.Veil.Handlebars
{
    public class VeilHandlebarsViewEngine : IViewEngine
    {
        private const string EngineKey = "handlebars";
        private IVeilContext context;
        private IVeilEngine engine;

        static VeilHandlebarsViewEngine()
        {
            VeilEngine.RegisterParser(EngineKey, () => new HandlebarsParser());
        }

        public IEnumerable<string> Extensions
        {
            get { return new[] { "haml" }; }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.context = new NancyVeilContext(viewEngineStartupContext.ViewLocator);
            this.engine = new VeilEngine(this.context);
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            var template = renderContext.ViewCache.GetOrAdd(viewLocationResult, result =>
            {
                Type modelType = model.GetType();
                return this.engine.CompileNonGeneric(EngineKey, result.Contents(), modelType);
            });

            var response = new Response();
            response.Contents = s =>
            {
                var writer = new StreamWriter(s);
                template(writer, model);
                writer.Flush();
            };
            return response;
        }
    }
}