using System;
using System.Collections.Generic;
using SuperSimpleViewEngine;

namespace Veil.Benchmark.Hosts
{
    public class SuperSimpleHost : IViewEngineHost
    {
        private Dictionary<string, string> registeredTemplates = new Dictionary<string, string>();

        public void RegisterTemplate(string name, string content)
        {
            registeredTemplates.Add(name, content);
        }

        public string AntiForgeryToken()
        {
            return "";
        }

        public object Context
        {
            get { return this; }
        }

        public string ExpandPath(string path)
        {
            throw new NotImplementedException();
        }

        public string GetTemplate(string templateName, object model)
        {
            return registeredTemplates[templateName];
        }

        public string GetUriString(string name, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public string HtmlEncode(string input)
        {
            return HttpEncoder.HtmlEncode(input);
        }
    }
}