using System;
using System.Collections.Generic;
using System.IO;
using Nancy.Bootstrapper;
using Veil;

namespace Nancy.ViewEngines.Veil
{
    public class VeilViewEngine : IViewEngine
    {
        private IVeilContext context;
        private IVeilEngine engine;
        private static List<string> supportedExtensions = new List<string>();

        static VeilViewEngine()
        {
            FindAndRegisterParsers();
        }

        private static void FindAndRegisterParsers()
        {
            foreach (var parserRegistrationType in AppDomainAssemblyTypeScanner.TypesOf<INancyVeilViewEngineRegistration>())
            {
                var parserRegistration = (INancyVeilViewEngineRegistration)Activator.CreateInstance(parserRegistrationType);
                foreach (var ext in parserRegistration.Extensions)
                {
                    VeilEngine.RegisterParser(ext, parserRegistration.ParserFactory);
                    supportedExtensions.Add(ext);
                }
            }
        }

        public IEnumerable<string> Extensions
        {
            get { return supportedExtensions; }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.context = new NancyVeilContext(viewEngineStartupContext.ViewLocator, supportedExtensions);
            this.engine = new VeilEngine(this.context);
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            var template = renderContext.ViewCache.GetOrAdd(viewLocationResult, result =>
            {
                Type modelType = model.GetType();
                return this.engine.CompileNonGeneric(viewLocationResult.Extension, result.Contents(), modelType);
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