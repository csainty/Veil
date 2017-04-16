using System.IO;
using Nancy;

namespace Veil.NancyViewEngines
{
    internal class TestingRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Path.Combine(Microsoft.DotNet.InternalAbstractions.ApplicationEnvironment.ApplicationBasePath, "NancyViewEngines");
        }
    }
}