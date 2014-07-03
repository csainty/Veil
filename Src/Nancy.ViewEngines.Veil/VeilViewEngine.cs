using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
                try
                {
                    var context = new NancyVeilContext(renderContext, Extensions);
                    var engine = new VeilEngine(context);
                    Type modelType = model == null ? typeof(object) : model.GetType();
                    return engine.CompileNonGeneric(viewLocationResult.Extension, result.Contents(), modelType);
                }
                catch (Exception e)
                {
                    return CreateErrorPage(e);
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

        private static Action<TextWriter, object> CreateErrorPage(Exception e)
        {
            return (writer, model) =>
            {
                var message = GetErrorMessage(e);
                writer.Write(ErrorTemplate.Value.Replace("[DETAILS]", message));
            };
        }

        private static string GetErrorMessage(Exception e)
        {
            if (StaticConfiguration.DisableErrorTraces) return "Error details are currently disabled. Please set <code>StaticConfiguration.DisableErrorTraces = false;</code> to enable.";
            if (e is TargetInvocationException) return GetErrorMessage(e.InnerException);
            if (e is VeilParserException) return e.Message;
            if (e is VeilCompilerException) return e.Message;
            return e.ToString();
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