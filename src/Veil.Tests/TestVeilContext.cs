using System.Collections.Generic;
using System.IO;

namespace Veil
{
    internal class TestVeilContext : IVeilContext
    {
        private readonly Dictionary<string, string> registeredTemplates = new Dictionary<string, string>();

        public TextReader GetTemplateByName(string name, string parserKey)
        {
            return new StringReader(registeredTemplates[name]);
        }

        public void RegisterTemplate(string name, string content)
        {
            registeredTemplates.Add(name, content);
        }
    }
}