using System.Collections.Generic;
using System.IO;

namespace Veil.Benchmark.Hosts
{
    public class VeilContext : IVeilContext
    {
        private Dictionary<string, string> registeredTemplates = new Dictionary<string, string>();

        public void RegisterTemplate(string name, string content)
        {
            registeredTemplates.Add(name, content);
        }

        public TextReader GetTemplateByName(string name, string parserKey)
        {
            return new StringReader(registeredTemplates[name]);
        }
    }
}