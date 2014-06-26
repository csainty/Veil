using Nancy.Testing;
using Nancy.ViewEngines.Veil;
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
            var browser = new Browser(with =>
            {
                with.Module<TestingModule>();
                with.ViewEngine<VeilViewEngine>();
                with.RootPathProvider<TestingRootPathProvider>();
            });
            var response = browser.Get(enginePath);

            response.Body["h1"].ShouldExistOnce().And.ShouldContain("Hello Joe");
            response.Body["header"].ShouldExistOnce();
        }
    }
}