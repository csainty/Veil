using System.IO;
using Nancy;

namespace Veil.NancyViewEngines
{
    internal class TestingRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Path.Combine(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "..", "NancyViewEngines");
        }
    }
}