using System;
using System.IO;
using Nancy;

namespace Veil.NancyViewEngines
{
    internal class TestingRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "..", "..", "NancyViewEngines");
        }
    }
}