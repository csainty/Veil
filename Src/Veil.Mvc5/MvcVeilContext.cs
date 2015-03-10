using System.IO;

namespace Veil.Mvc5
{
    internal class MvcVeilContext : IVeilContext
    {
        private VeilViewEngine ViewEngine;
        private readonly string ControllerName;

        public MvcVeilContext(VeilViewEngine viewEngine, string controllerName)
        {
            ViewEngine = viewEngine;
            ControllerName = controllerName;
        }

        public TextReader GetTemplateByName(string name, string parserKey)
        {
            var viewFile = ViewEngine.LocateView(ControllerName, name, parserKey);

            if (viewFile == null)
            {
                return null;
            }

            return new StreamReader(viewFile.Open());
        }
    }
}