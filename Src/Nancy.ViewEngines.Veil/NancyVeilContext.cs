using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veil;

namespace Nancy.ViewEngines.Veil
{
    internal class NancyVeilContext : IVeilContext
    {
        private readonly IDictionary<string, ViewLocationResult> views = new Dictionary<string, ViewLocationResult>();

        public NancyVeilContext(IViewLocator locator, IEnumerable<string> extensions)
        {
            var extensionSet = new HashSet<string>(extensions);
            var veilViews = locator
                .GetAllCurrentlyDiscoveredViews()
                .Where(x => extensionSet.Contains(x.Extension));

            foreach (var view in veilViews)
            {
                views.Add(view.Name + "." + view.Extension, view);
                if (!views.ContainsKey(view.Name))
                {
                    views.Add(view.Name, view);
                }
            }
        }

        public TextReader GetTemplateByName(string name, string templateType)
        {
            ViewLocationResult view;
            if (!views.TryGetValue(name + "." + templateType, out view) && !views.TryGetValue(name, out view)) return null;

            return view.Contents();
        }
    }
}