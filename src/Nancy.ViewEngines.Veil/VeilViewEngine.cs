using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nancy.Responses;
using Veil;

namespace Nancy.ViewEngines.Veil
{
    public class VeilViewEngine : IViewEngine
    {
        private static List<string> supportedExtensions = new List<string>();

        static VeilViewEngine()
        {
            supportedExtensions.AddRange(VeilStaticConfiguration.RegisteredParserKeys);
        }

        public IEnumerable<string> Extensions
        {
            get { return supportedExtensions; }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            var template = renderContext.ViewCache.GetOrAdd(viewLocationResult, result =>
            {
                using (var contents = result.Contents())
                {
                    var context = new NancyVeilContext(renderContext, Extensions);
                    var engine = new VeilEngine(context);
                    Type modelType = model == null ? typeof(object) : model.GetType();
                    return engine.CompileNonGeneric(viewLocationResult.Extension, contents, modelType);
                }
            });

            var response = new HtmlResponse();
            response.ContentType = "text/html; charset=utf-8";
            response.Contents = s =>
            {
                var writer = new StreamWriter(s, Encoding.UTF8);
                template(writer, model);
                writer.Flush();
            };
            return response;
        }

    }
}