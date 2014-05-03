using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Veil.Handlebars;
using Veil.SuperSimple;

namespace Veil
{
    [TestFixture]
    public class VeilEngineTests
    {
        private TestVeilContext context;
        private IVeilEngine engine;

        private ViewModel viewModel = new ViewModel
        {
            Name = "Chris",
            ViewCount = 10,
            IsAdmin = false,
            Department = new Department
            {
                DepartmentName = "Developers",
                Company = new Company
                {
                    CompanyName = "Veil"
                }
            },
            Roles = new[] { "User", "Browser" }
        };

        [SetUp]
        public void SetUp()
        {
            VeilEngine.ClearParserRegistrations();
            VeilEngine.RegisterParser("handlebars", new HandlebarsParser());
            VeilEngine.RegisterParser("supersimple", new SuperSimpleParser());

            context = new TestVeilContext();
            engine = new VeilEngine(context);
        }

        [TestCaseSource("HandlebarsTemplates")]
        public void Should_render_handlebars_template(string template, string expectedResult)
        {
            var view = Compile(template, "handlebars");
            var result = Execute(view, viewModel);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("SuperSimpleTemplates")]
        public void Should_render_supersimple_template(string template, string expectedResult)
        {
            context.RegisterTemplate("Role", "@Current;");
            context.RegisterTemplate("Roles", "<ul>@Each.Current;<li>@Partial['Role'];</li>@EndEach;</ul>");
            context.RegisterTemplate("Department", "@Model.DepartmentName @Model.Company.CompanyName");
            var view = Compile(template, "supersimple");
            var result = Execute(view, viewModel);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        public object[] HandlebarsTemplates()
        {
            return new object[] {
                new object[] {"Hello {{ Name }}. You have visited us {{ ViewCount }} times!", "Hello Chris. You have visited us 10 times!"},
                new object[] {"{{#if Name}}Hello {{Name}}{{/if}}", "Hello Chris"},
                new object[] {"{{#if ViewCount}}Count: {{ViewCount}}{{/if}}", "Count: 10"},
                new object[] {"{{#if IsAdmin}}Yo Admin!{{/if}}", ""},
                new object[] {"{{#if IsAdmin}}Yo Admin!{{else}}Sorry{{/if}}", "Sorry"},
                new object[] {"Hey {{ Name }}, {{ Department.DepartmentName }} {{#if Department.Company }}{{ Department.Company.CompanyName }}{{/if}}", "Hey Chris, Developers Veil"},
                new object[] {"Department: {{ Department.GetDepartmentNumber() }}", "Department: 10"},
                new object[] {"Hey {{ Name }}, You are in roles{{#each Roles}} {{ this }}{{/each}}", "Hey Chris, You are in roles User Browser"}
            };
        }

        public object[] SuperSimpleTemplates()
        {
            return new object[] {
                new object[] {"Hello @Model.Name;. You have visited us @Model.ViewCount times!", "Hello Chris. You have visited us 10 times!"},
                new object[] {"@If.Name;Hello @Model.Name;@EndIf", "Hello Chris"},
                new object[] {"@If.ViewCount;Count: @Model.ViewCount;@EndIf", "Count: 10"},
                new object[] {"@If.IsAdmin;Yo Admin!@EndIf", ""},
                new object[] {"@If.IsAdmin;Yo Admin!@EndIf;@IfNot.IsAdmin;Sorry@EndIf", "Sorry"},
                new object[] {"Hey @Name;, @Department.DepartmentName; @If.Department.Company;@Model.Department.Company.CompanyName;@EndIf", "Hey Chris, Developers Veil"},
                new object[] {"Hey @Name, You are in roles@Each.Roles; @Current;@EndEach;", "Hey Chris, You are in roles User Browser"},
                new object[] {"Hey @Name, You are in roles @Each.Roles;@Partial['Role'];@EndEach;", "Hey Chris, You are in roles UserBrowser"},
                new object[] {"Hey @Name, You are in roles @Partial['Roles', Model.Roles];", "Hey Chris, You are in roles <ul><li>User</li><li>Browser</li></ul>"},
                new object[] {"Hey @Name from @Partial['Department', Department]", "Hey Chris from Developers Veil"}
            };
        }

        private Action<TextWriter, ViewModel> Compile(string template, string templateType)
        {
            using (var reader = new StringReader(template))
            {
                return engine.Compile<ViewModel>(templateType, reader);
            }
        }

        private string Execute(Action<TextWriter, ViewModel> view, ViewModel model)
        {
            using (var writer = new StringWriter())
            {
                view(writer, model);
                return writer.ToString();
            }
        }

        private class ViewModel
        {
            public string Name { get; set; }

            public int ViewCount { get; set; }

            public bool IsAdmin = false;

            public Department Department { get; set; }

            public IEnumerable<string> Roles { get; set; }
        }

        private class Department
        {
            public string DepartmentName { get; set; }

            public Company Company { get; set; }

            public int GetDepartmentNumber()
            {
                return 10;
            }
        }

        private class Company
        {
            public string CompanyName { get; set; }
        }
    }
}