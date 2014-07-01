using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veil;

namespace Nancy.ViewEngines.Veil
{
    internal class NancyVeilContext : IVeilContext
    {
        private readonly IRenderContext context;
        private readonly string[] extensions;

        public NancyVeilContext(IRenderContext context, IEnumerable<string> extensions)
        {
            this.context = context;
            this.extensions = extensions.ToArray();
        }

        public TextReader GetTemplateByName(string name, string parserKey)
        {
            var view = this.context.LocateView(name + "." + parserKey, null);
            if (view == null)
            {
                view = this.context.LocateView(name, null);
            }
            return view == null ? null : view.Contents();
        }
    }
}