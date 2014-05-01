using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veil;

namespace Nancy.ViewEngines.Veil.SuperSimple
{
    internal class NancyVeilContext : IVeilContext
    {
        private readonly IDictionary<string, ViewLocationResult> views;

        public NancyVeilContext(IViewLocator locator)
        {
            this.views = locator.GetAllCurrentlyDiscoveredViews().ToDictionary(x => x.Name);
        }

        public TextReader GetTemplateByName(string name, string templateType)
        {
            if (!views.ContainsKey(name)) return null;

            return views[name].Contents();
        }
    }
}