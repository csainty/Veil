using System.Collections.Generic;
using System.Dynamic;
using NUnit.Framework;
using E = Veil.Parser.Expression;
using S = Veil.Parser.SyntaxTree;

namespace Veil.Compiler
{
    [TestFixture]
    internal class LateBoundTests : CompilerTestBase
    {
        [TestCaseSource("TestCases")]
        public void Should_handle_late_binding<T>(T model)
        {
            var template = S.Block(
                S.WriteExpression(E.LateBound("Name"))
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Joe"));
        }

        [Test]
        public void Should_not_get_mixed_up_by_caching_of_late_bindings()
        {
            var model = new Dictionary<string, object>();
            model.Add("Name", "D1");
            model.Add("User", new { Name = "U" });
            model.Add("Department", new Dictionary<string, object> { { "Name", "D2" } });
            var template = S.Block(
                S.WriteExpression(E.LateBound("Name")),
                S.WriteExpression(E.SubModel(E.LateBound("User"), E.LateBound("Name"))),
                S.WriteExpression(E.SubModel(E.LateBound("Department"), E.LateBound("Name")))
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("D1UD2"));
        }

        public object[] TestCases()
        {
            return new object[] {
                new object[] { new { Name = "Joe" } },
                new object[] { new ViewModel() },
                new object[] { new Dictionary<string, object> { { "Name", "Joe" } } },
                new object[] { new Dictionary<string, string> { { "Name", "Joe" } } },
                new object[] { Expando() }
            };
        }

        private dynamic Expando()
        {
            dynamic model = new ExpandoObject();
            model.Name = "Joe";
            return model;
        }

        private class ViewModel
        {
            public string Name = "Joe";
        }
    }
}