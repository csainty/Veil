using System;
using System.IO;
using System.Web.Mvc;

namespace Veil.Mvc5
{
    internal class VeilView : IView
    {
        private readonly Action<TextWriter, object> template;

        public VeilView(Action<TextWriter, object> template)
        {
            this.template = template;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            template.Invoke(writer, viewContext.ViewData.Model);
        }
    }
}