using System;
using System.Collections.Generic;
using System.IO;
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
            supportedExtensions.AddRange(VeilStaticConfiguration.RegisteredParserKeys);
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
                try
                {
                    Type modelType = model == null ? typeof(object) : model.GetType();
                    return this.engine.CompileNonGeneric(viewLocationResult.Extension, result.Contents(), modelType);
                }
                catch (Exception e)
                {
                    return CreateErrorPage(e);
                }
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

        private static Action<TextWriter, object> CreateErrorPage(Exception e)
        {
            return (writer, model) =>
            {
                var message = StaticConfiguration.DisableErrorTraces ? "Error details are currently disabled. Please set <code>StaticConfiguration.DisableErrorTraces = false;</code> to enable." : e.ToString();
                writer.Write(ErrorTemplate.Value.Replace("[DETAILS]", e.ToString()));
            };
        }

        private static Lazy<string> ErrorTemplate = new Lazy<string>(() =>
        {
            var resourceStream = typeof(VeilViewEngine).Assembly.GetManifestResourceStream("Nancy.ViewEngines.Veil.Resources.CompilationError.html");

            if (resourceStream == null)
            {
                return string.Empty;
            }

            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        });
    }
}