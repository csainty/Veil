using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veil
{
    internal class TestVeilContext : IVeilContext
    {
        public System.IO.TextReader GetTemplateByName(string name, string templateType)
        {
            throw new NotImplementedException();
        }
    }
}
