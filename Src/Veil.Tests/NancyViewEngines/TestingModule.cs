using Nancy;

namespace Veil.NancyViewEngines
{
    internal class TestingModule : NancyModule
    {
        public TestingModule()
        {
            Get["/supersimple"] = _ =>
            {
                return View["SuperSimple", new IndexViewModel { Name = "Joe" }];
            };

            Get["/handlebars"] = _ =>
            {
                return View["Handlebars", new IndexViewModel { Name = "Joe" }];
            };
        }
    }
}