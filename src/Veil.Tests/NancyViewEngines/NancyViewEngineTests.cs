using Nancy.Testing;
using Nancy.ViewEngines.Veil;
using Veil.Handlebars;
using Veil.SuperSimple;
using Xunit;

namespace Veil.NancyViewEngines
{
    public class NancyViewEngineTests
    {
        static NancyViewEngineTests() {
            VeilStaticConfiguration.RegisterParser(new HandlebarsTemplateParserRegistration());
            VeilStaticConfiguration.RegisterParser(new SuperSimpleParserRegistration());
        }

        [Theory]
        [InlineData("/supersimple")]
        [InlineData("/handlebars")]
        public void Should_render_templates(string enginePath)
        {
            var browser = new Browser(with =>
            {
                with.Module<TestingModule>();
                with.ViewEngine<VeilViewEngine>();
                with.RootPathProvider<TestingRootPathProvider>();
            });
            var response = browser.Get(enginePath).Result;

            response.Body["h1"].ShouldExistOnce().And.ShouldContain("Hello Joe");
            response.Body["header"].ShouldExistOnce();
        }
    }
}