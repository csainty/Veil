using System;
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
            public string GetRootPath()
            {
                return Path.Combine(Environment.CurrentDirectory, "..", "..", "NancyViewEngines");
            }
        }
    }
}