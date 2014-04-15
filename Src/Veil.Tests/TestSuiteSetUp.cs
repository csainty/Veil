using NUnit.Framework;
using Veil.Handlebars;

namespace Veil
{
    [SetUpFixture]
    public class TestSuiteSetUp
    {
        [SetUp]
        public void SetUp()
        {
            VeilEngine.RegisterParser("handlebars", new HandlebarsParser());
        }
    }
}