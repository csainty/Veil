using System.Collections.Generic;
using System.Dynamic;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    [TestFixture]
    internal class LateBoundTests : CompilerTestBase
    {
        [TestCaseSource("TestCases")]
        public void Should_handle_late_binding<T>(T model)
        {
            var template = SyntaxTreeNode.Block(
                SyntaxTreeNode.WriteExpression(SyntaxTreeNode.ExpressionNode.LateBound("Name"))
            );
            var result = ExecuteTemplate(template, model);
            Assert.That(result, Is.EqualTo("Joe"));
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