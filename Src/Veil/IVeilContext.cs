using System.IO;

namespace Veil
{
    public interface IVeilContext
    {
        TextReader GetTemplateByName(string name, string templateType);
    }
}