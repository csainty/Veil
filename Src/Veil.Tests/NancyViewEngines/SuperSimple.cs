using System.IO;
using Nancy;
using Nancy.Testing;
using Nancy.ViewEngines.Veil.SuperSimple;
using NUnit.Framework;

namespace Veil.NancyViewEngines
{
    [TestFixture]
    public class SuperSimple
    {
        [Test]
        public void Should_render_super_simple_template()
        {
            var browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                with.Module(new TestingModule());
                with.RootPathProvider(new TestingRootPathProvider());
                with.ViewEngine(new VeilSuperSimpleViewEngine());
            }));
            var response = browser.Get("/");

            response.Body["h1"].ShouldExistOnce().And.ShouldContain("Hello Joe");
            response.Body["header"].ShouldExistOnce();
        }

        private class IndexViewModel
        {
            public string Name { get; set; }
        }

        private class TestingModule : NancyModule
        {
            public TestingModule()
            {
                Get["/"] = _ =>
                {
                    return View["Index", new IndexViewModel { Name = "Joe" }];
                };
            }
        }

        private class TestingRootPathProvider : IRootPathProvider
        {
            private static readonly string RootPath;

            static TestingRootPathProvider()
            {
                var directoryName = Path.GetDirectoryName(typeof(SuperSimple).Assembly.CodeBase);

                if (directoryName != null)
                {
                    var assemblyPath = directoryName.Replace(@"file:\", string.Empty);

                    RootPath = Path.GetFullPath(Path.Combine(assemblyPath, "..", "..", "NancyViewEngines"));
                }
            }

            public string GetRootPath()
            {
                return RootPath;
            }
        }
    }
}