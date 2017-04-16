using System.Collections.Generic;
using System.Dynamic;
using Veil.Parser;
using Xunit;

namespace Veil.Compiler
{
    
    public class LateBoundTests : CompilerTestBase
    {
        [Theory]
        [MemberData("TestCases")]
        public void Should_handle_late_binding<T>(T model)
        {
            var template = SyntaxTree.Block(
                SyntaxTree.WriteExpression(SyntaxTreeExpression.LateBound("Name"))
            );
            var result = ExecuteTemplate(template, model);
            Assert.Equal("Joe", result);
        }

        [Fact]
        public void Should_not_get_mixed_up_by_caching_of_late_bindings()
        {
            var model = new Dictionary<string, object>();
            model.Add("Name", "D1");
            model.Add("User", new { Name = "U" });
            model.Add("Department", new Dictionary<string, object> { { "Name", "D2" } });
            var template = SyntaxTree.Block(
                SyntaxTree.WriteExpression(SyntaxTreeExpression.LateBound("Name")),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.SubModel(SyntaxTreeExpression.LateBound("User"), SyntaxTreeExpression.LateBound("Name"))),
                SyntaxTree.WriteExpression(SyntaxTreeExpression.SubModel(SyntaxTreeExpression.LateBound("Department"), SyntaxTreeExpression.LateBound("Name")))
            );
            var result = ExecuteTemplate(template, model);
            Assert.Equal("D1UD2", result);
        }

        public static object[] TestCases()
        {
            return new object[] {
                new object[] { new { Name = "Joe" } },
                new object[] { new ViewModel() },
                new object[] { new Dictionary<string, object> { { "Name", "Joe" } } },
                new object[] { new Dictionary<string, string> { { "Name", "Joe" } } },
                new object[] { Expando() }
            };
        }

        private static dynamic Expando()
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