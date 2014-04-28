using NUnit.Framework;
using Veil.Handlebars;
using Veil.SuperSimple;

namespace Veil
{
    [SetUpFixture]
    public class TestSuiteSetUp
    {
        [SetUp]
        public void SetUp()
        {
            VeilEngine.RegisterParser("handlebars", new HandlebarsParser());
            VeilEngine.RegisterParser("supersimple", new SuperSimpleParser());
        }
    }
}