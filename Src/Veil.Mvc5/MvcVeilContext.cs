using System.IO;

namespace Veil.Mvc5
{
    internal class MvcVeilContext : IVeilContext
    {
        private VeilViewEngine ViewEngine;
        private readonly string ControllerName;
        private readonly string AreaName;

        public MvcVeilContext(VeilViewEngine viewEngine, string areaName, string controllerName)
        {
            ViewEngine = viewEngine;
            AreaName = areaName;
            ControllerName = controllerName;
        }

        public TextReader GetTemplateByName(string name, string parserKey)
        {
            var viewFile = ViewEngine.LocateView(AreaName, ControllerName, name, parserKey);

            if (viewFile == null)
            {
                return null;
            }

            return new StreamReader(viewFile.Open());
        }
    }
}