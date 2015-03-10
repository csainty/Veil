using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Veil.Mvc5
{
    public class VeilViewEngine : IViewEngine
    {
        /// <summary>
        /// An array of locations to search for views.
        /// These are formatted as String.Format("[format]", viewName, controllerName, fileExtension)
        /// </summary>
        public string[] ViewLocationFormats { get; set; }

        private string[] Extensions;

        private static ConcurrentDictionary<string, VeilView> ViewCache;

        public VeilViewEngine()
        {
            Extensions = VeilStaticConfiguration.RegisteredParserKeys.ToArray();
            ViewCache = new ConcurrentDictionary<string, VeilView>();
            ViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.{2}",
                "~/Views/Shared/{0}.{2}"
            };
        }

        public VirtualFile LocateView(string controllerName, string viewName)
        {
            return Extensions
                .Select(x => LocateView(controllerName, viewName, x))
                .Where(x => x != null)
                .FirstOrDefault();
        }

        public VirtualFile LocateView(string controllerName, string viewName, string extension)
        {
            return ViewLocationFormats
                .Select(x => String.Format(x, viewName, controllerName, extension))
                .Where(HostingEnvironment.VirtualPathProvider.FileExists)
                .Select(HostingEnvironment.VirtualPathProvider.GetFile)
                .FirstOrDefault();
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            throw new NotSupportedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var view = GetOrCompileView(controllerName, viewName, controllerContext.Controller.ViewData.Model.GetType(), useCache);

            if (view == null)
            {
                return null;
            }

            return new ViewEngineResult(view, this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var d = view as IDisposable;
            if (d != null)
            {
                d.Dispose();
            }
        }

        private VeilView GetOrCompileView(string controllerName, string viewName, Type modelType, bool useCache)
        {
            if (!useCache || (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled))
            {
                return CompileView(controllerName, viewName, modelType);
            }

            var cacheKey = String.Format("VeilViewEngine:CompileView:{0}:{1}:{2}", controllerName, viewName, modelType.FullName);
            return ViewCache.GetOrAdd(cacheKey, x => CompileView(controllerName, viewName, modelType));
        }

        private VeilView CompileView(string controllerName, string viewName, Type modelType)
        {
            var viewFile = LocateView(controllerName, viewName);

            if (viewFile == null)
            {
                return null;
            }

            using (var contents = new StreamReader(viewFile.Open()))
            {
                var ctx = new MvcVeilContext(this, controllerName);
                var template = new VeilEngine(ctx).CompileNonGeneric(
                    Path.GetExtension(viewFile.Name).TrimStart('.'),
                    contents,
                    modelType
                );
                return new VeilView(template);
            }
        }
    }
}