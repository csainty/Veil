using Nancy.Testing;
using Nancy.ViewEngines.Veil.Handlebars;
using Nancy.ViewEngines.Veil.SuperSimple;
using NUnit.Framework;

namespace Veil.NancyViewEngines
{
    [TestFixture]
    public class NancyViewEngineTests
    {
        [TestCase("/supersimple")]
        [TestCase("/handlebars")]
        public void Should_render_templates(string enginePath)
        {
            var browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                with.Module(new TestingModule());
                with.RootPathProvider(new TestingRootPathProvider());
                with.ViewEngine(new VeilSuperSimpleViewEngine());
                with.ViewEngine(new VeilHandlebarsViewEngine());
            }));
            var response = browser.Get(enginePath);

            response.Body["h1"].ShouldExistOnce().And.ShouldContain("Hello Joe");
            response.Body["header"].ShouldExistOnce();
        }
    }
}