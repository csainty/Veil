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
        /// These are formatted as String.Format("[format]", viewName, controllerName, "",  fileExtension)
        /// </summary>
        public string[] ViewLocationFormats { get; set; }

        /// <summary>
        /// An array of locations to search for views when route contains an area
        /// /// These are formatted as String.Format("[format]", viewName, controllerName, areaName, fileExtension)
        /// </summary>
        public string[] AreaLocationFormats { get; set; }

        private string[] Extensions;

        private static ConcurrentDictionary<string, VeilView> ViewCache;

        public VeilViewEngine()
        {
            Extensions = VeilStaticConfiguration.RegisteredParserKeys.ToArray();
            ViewCache = new ConcurrentDictionary<string, VeilView>();
            ViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.{3}",
                "~/Views/Shared/{0}.{3}"
            };
            AreaLocationFormats = new[]
            {
                "~/Areas/{2}/Views/{1}/{0}.{3}",
                "~/Areas/{2}/Views/Shared/{0}.{3}"
            };
        }

        public VirtualFile LocateView(string areaName, string controllerName, string viewName)
        {
            return Extensions
                .Select(x => LocateView(areaName, controllerName, viewName, x))
                .Where(x => x != null)
                .FirstOrDefault();
        }

        public VirtualFile LocateView(string areaName, string controllerName, string viewName, string extension)
        {
            var formats = String.IsNullOrEmpty(areaName) ? ViewLocationFormats : AreaLocationFormats;
            return formats
                .Select(x => String.Format(x, viewName, controllerName, areaName, extension))
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
            var areaName = controllerContext.RouteData.DataTokens["area"] as string;
            var modelType = controllerContext.Controller.ViewData.Model == null ? typeof(object) : controllerContext.Controller.ViewData.Model.GetType();
            var view = GetOrCompileView(areaName, controllerName, viewName, modelType, useCache);

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

        private VeilView GetOrCompileView(string areaName, string controllerName, string viewName, Type modelType, bool useCache)
        {
            if (!useCache || (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled))
            {
                return CompileView(areaName, controllerName, viewName, modelType);
            }

            var cacheKey = String.Format("VeilViewEngine:CompileView:{0}:{1}:{2}:{3}", areaName, controllerName, viewName, modelType.FullName);
            return ViewCache.GetOrAdd(cacheKey, x => CompileView(areaName, controllerName, viewName, modelType));
        }

        private VeilView CompileView(string areaName, string controllerName, string viewName, Type modelType)
        {
            var viewFile = LocateView(areaName, controllerName, viewName);

            if (viewFile == null)
            {
                return null;
            }

            using (var contents = new StreamReader(viewFile.Open()))
            {
                var ctx = new MvcVeilContext(this, areaName, controllerName);
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