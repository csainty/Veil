using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using NUnit.Framework;

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
            Roles = new[] { "User", "Browser" },
            Reports = new string[0],
            Properties = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                { "EnableFoo", "True" }
            })
        };

        [SetUp]
        public void SetUp()
        {
            context = new TestVeilContext();
            engine = new VeilEngine(context);
        }

        [TestCaseSource("HandlebarsTemplates")]
        public void Should_render_handlebars_template(string template, string expectedResult)
        {
            RegisterHandlebarsTemplates();
            var view = Compile(template, "handlebars");
            var result = Execute(view, viewModel);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("HandlebarsTemplates")]
        public void Should_render_handlebars_template_nongeneric(string template, string expectedResult)
        {
            RegisterHandlebarsTemplates();
            var view = CompileNonGeneric(template, "handlebars");
            var result = Execute(view, viewModel);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("HandlebarsTemplates")]
        public void Should_render_handlebars_template_latebound(string template, string expectedResult)
        {
            RegisterHandlebarsTemplates();
            var view = CompileLateBound(template, "handlebars");
            var result = Execute(view, viewModel);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("SuperSimpleTemplates")]
        public void Should_render_supersimple_template(string template, string expectedResult)
        {
            RegisterSuperSimpleTemplates();
            var view = Compile(template, "supersimple");
            var result = Execute(view, viewModel);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("SuperSimpleTemplates")]
        public void Should_render_supersimple_template_nongeneric(string template, string expectedResult)
        {
            RegisterSuperSimpleTemplates();
            var view = CompileNonGeneric(template, "supersimple");
            var result = Execute(view, viewModel);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCaseSource("SuperSimpleTemplates")]
        public void Should_render_supersimple_template_latebound(string template, string expectedResult)
        {
            RegisterSuperSimpleTemplates();
            var view = CompileLateBound(template, "supersimple");
            var result = Execute(view, viewModel);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Should_work_with_no_veilcontext()
        {
            var view = new VeilEngine().Compile<ViewModel>("supersimple", new StringReader("Hello @Model.Name"));
            using (var writer = new StringWriter())
            {
                view(writer, new ViewModel { Name = "Joe" });
                Assert.That(writer.ToString(), Is.EqualTo("Hello Joe"));
            }
        }

        [Test]
        public void Should_throw_if_attempt_to_use_partials_without_veilcontext()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var view = new VeilEngine().Compile<ViewModel>("supersimple", new StringReader("Hello @Partial['Person'];"));
            });
        }

        public object[] HandlebarsTemplates()
        {
            return new object[] {
                new object[] {"Hello {{ Name }}. You have visited us {{ ViewCount }} times!", "Hello Chris. You have visited us 10 times!"},
                new object[] {"Hello {{ name }}. You have visited us {{ viewCount }} times!", "Hello Chris. You have visited us 10 times!"},
                new object[] {"{{#if Name}}Hello {{Name}}{{/if}}", "Hello Chris"},
                new object[] {"{{#if HasViewCount}}Count: {{ViewCount}}{{/if}}", "Count: 10"},
                new object[] {"{{#if IsAdmin}}Yo Admin!{{/if}}", ""},
                new object[] {"{{#if IsAdmin}}Yo Admin!{{else}}Sorry{{/if}}", "Sorry"},
                new object[] {"Hey {{ Name }}, {{ Department.DepartmentName }} {{#if Department.Company }}{{ Department.Company.CompanyName }}{{/if}}", "Hey Chris, Developers Veil"},
                new object[] {"Department: {{ Department.GetDepartmentNumber() }}", "Department: 10"},
                new object[] {"Hey {{ Name }}, You are in roles{{#each Roles}} {{ this }}{{/each}}", "Hey Chris, You are in roles User Browser"},
                new object[] {"Hey {{ Name }}, You are in roles{{#each Roles}} {{> role}}{{/each}}", "Hey Chris, You are in roles User Browser"},
                new object[] {"Hey {{ Name }},{{#each Roles}} {{ ../Name }}{{/each}}", "Hey Chris, Chris Chris"},
                new object[] {"<p>\r\n{{~Name~}}\r\n</p>", "<p>Chris</p>"},
                new object[] {"{{#with Department}}Department: {{ GetDepartmentNumber() }}{{/with}}", "Department: 10"},
                new object[] {"{{#each Reports}}{{this}}{{else}}No Reports{{/each}}", "No Reports"},
                new object[] {"{{< master}}Testing 1.2.3", "Hello Chris Testing 1.2.3 See Ya!"},
                new object[] {"{{#with Department}}{{../Name}}: {{DepartmentName}}{{/with}}", "Chris: Developers"},
                new object[] {"{{ Properties.EnableFoo }}", "True"},
                new object[] {"{{ Department.Properties.HasManager }}", "False"}
            };
        }

        public object[] SuperSimpleTemplates()
        {
            return new object[] {
                new object[] {"Hello @Model.Name;. You have visited us @Model.ViewCount times!", "Hello Chris. You have visited us 10 times!"},
                new object[] {"@If.Name;Hello @Model.Name;@EndIf", "Hello Chris"},
                new object[] {"@If.HasViewCount;Count: @Model.ViewCount;@EndIf", "Count: 10"},
                new object[] {"@If.IsAdmin;Yo Admin!@EndIf", ""},
                new object[] {"@If.IsAdmin;Yo Admin!@EndIf;@IfNot.IsAdmin;Sorry@EndIf", "Sorry"},
                new object[] {"Hey @Model.Name;, @Model.Department.DepartmentName; @If.Department.Company;@Model.Department.Company.CompanyName;@EndIf", "Hey Chris, Developers Veil"},
                new object[] {"Hey @Model.Name, You are in roles@Each.Roles; @Current;@EndEach;", "Hey Chris, You are in roles User Browser"},
                new object[] {"Hey @Model.Name, You are in roles @Each.Roles;@Partial['Role'];@EndEach;", "Hey Chris, You are in roles UserBrowser"},
                new object[] {"Hey @Model.Name, You are in roles @Partial['Roles', Model.Roles];", "Hey Chris, You are in roles <ul><li>User</li><li>Browser</li></ul>"},
                new object[] {"Hey @Model.Name from @Partial['Department', Department]", "Hey Chris from Developers Veil"},
                new object[] {"@Master['Master']; @Section['Middle'];Testing 1.2.3@EndSection;", "Hello Chris Testing 1.2.3 See Ya!"},
                new object[] {"@Master['MiddleMaster']; @Section['Content'];Testing 1.2.3@EndSection;", "Hello Chris from Developers Testing 1.2.3 See Ya!"},
                new object[] {"@Model.Properties.EnableFoo", "True"},
                new object[] {"@Model.Department.Properties.HasManager", "False"}
            };
        }

        private void RegisterSuperSimpleTemplates()
        {
            context.RegisterTemplate("Role", "@Current;");
            context.RegisterTemplate("Roles", "<ul>@Each.Current;<li>@Partial['Role'];</li>@EndEach;</ul>");
            context.RegisterTemplate("Department", "@Model.DepartmentName @Model.Company.CompanyName");
            context.RegisterTemplate("Master", "Hello @Model.Name; @Section['Middle'] See Ya!");
            context.RegisterTemplate("MiddleMaster", "@Master['Master'] @Section['Middle']from @Model.Department.DepartmentName @Section['Content'];@EndSection");
        }

        private void RegisterHandlebarsTemplates()
        {
            context.RegisterTemplate("role", "{{ this }}");
            context.RegisterTemplate("master", "Hello {{ Name }} {{body}} See Ya!");
        }

        private Action<TextWriter, ViewModel> Compile(string template, string parserKey)
        {
            using (var reader = new StringReader(template))
            {
                return engine.Compile<ViewModel>(parserKey, reader);
            }
        }

        private Action<TextWriter, object> CompileNonGeneric(string template, string parserKey)
        {
            using (var reader = new StringReader(template))
            {
                return engine.CompileNonGeneric(parserKey, reader, typeof(ViewModel));
            }
        }

        private Action<TextWriter, object> CompileLateBound(string template, string parserKey)
        {
            using (var reader = new StringReader(template))
            {
                return engine.Compile<object>(parserKey, reader);
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

            public bool HasViewCount { get { return this.ViewCount > 0; } }

            public Department Department { get; set; }

            public IEnumerable<string> Roles { get; set; }

            public IEnumerable<string> Reports { get; set; }

            public IReadOnlyDictionary<string, string> Properties { get; set; }
        }

        private class Department
        {
            public string DepartmentName { get; set; }

            public Company Company { get; set; }

            public dynamic Properties { get; set; }

            public Department()
            {
                this.Properties = new ExpandoObject();
                this.Properties.HasManager = "False";
            }

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