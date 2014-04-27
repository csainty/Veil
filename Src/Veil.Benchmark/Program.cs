using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using RazorEngine;
using SimpleSpeedTester.Core;
using Veil.Handlebars;
using Veil.SuperSimple;

namespace Veil.Benchmark
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var model = new ViewModel
            {
                Name = "Test Template",
                IsLoggedIn = true,
                Roles = new[] { "User", "Admin", "Editor", "Viewer", "Uploader", "Pick & Pack" }
            };
            var handlebarsTemplate = ReadTemplate("haml");
            var razorTemplate = ReadTemplate("cshtml");
            var ssTemplate = ReadTemplate("sshtml");

            VeilEngine.RegisterParser("haml", new HandlebarsParser());
            VeilEngine.RegisterParser("sshtml", new SuperSimpleParser());
            var veilEngine = new VeilEngine();

            {
                var veilTemplate = veilEngine.Compile<ViewModel>("haml", new StringReader(handlebarsTemplate));
                AssertTemplateSample(Unwrap(veilTemplate, model), "Veil.Handlebars");
                var veilGroup = new TestGroup("Veil.Handlebars").PlanAndExecute("Template", () =>
                {
                    veilTemplate(new StringWriter(), model);
                }, 5000);
                Console.WriteLine(veilGroup);
                Console.WriteLine("---------");
            }

            using (var handlebars = new Chevron.Handlebars())
            {
                handlebars.RegisterTemplate("default", handlebarsTemplate);
                AssertTemplateSample(handlebars.Transform("default", model), "Chevron.IE.Merged");
                var chevronGroup = new TestGroup("Chevron.IE.Merged").PlanAndExecute("Template", () =>
                {
                    handlebars.Transform("default", model);
                }, 5000);
                Console.WriteLine(chevronGroup);
                Console.WriteLine("---------");
            }

            {
                Razor.Compile<ViewModel>(razorTemplate, "Test");
                AssertTemplateSample(Razor.Run<ViewModel>("Test", model), "Razor");
                var razorGroup = new TestGroup("Razor").PlanAndExecute("Template", () =>
                {
                    Razor.Run<ViewModel>("Test", model);
                }, 5000);
                Console.WriteLine(razorGroup);
                Console.WriteLine("---------");
            }

            {
                var veilTemplate = veilEngine.Compile<ViewModel>("sshtml", new StringReader(ssTemplate));
                AssertTemplateSample(Unwrap(veilTemplate, model), "Veil.SuperSimple");
                var ssGroup = new TestGroup("Veil.SuperSimple").PlanAndExecute("Template", () =>
                {
                    veilTemplate(new StringWriter(), model);
                }, 5000);
                Console.WriteLine(ssGroup);
                Console.WriteLine("---------");
            }

            {
                var engine = new SuperSimpleViewEngine.SuperSimpleViewEngine();
                var host = new TestHost();
                AssertTemplateSample(engine.Render(ssTemplate, model, host), "SuperSimpleViewEngine");
                var ssGroup = new TestGroup("SuperSimpleViewEngine").PlanAndExecute("Template", () =>
                {
                    engine.Render(ssTemplate, model, host);
                }, 5000);
                Console.WriteLine(ssGroup);
                Console.WriteLine("---------");
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static string Unwrap<T>(Action<TextWriter, T> template, T model)
        {
            using (var writer = new StringWriter())
            {
                template(writer, model);
                return writer.ToString();
            }
        }

        private static void AssertTemplateSample(string sample, string engine)
        {
            string expectedResult = ReadTemplate("txt");
            expectedResult = Regex.Replace(expectedResult, @"\s", "");
            sample = Regex.Replace(sample, @"\s", "");
            if (!String.Equals(expectedResult, sample))
            {
                Console.WriteLine("!!! -- Sample didn't match for test " + engine + " -- !!!");
            }
        }

        private static string ReadTemplate(string extension)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Veil.Benchmark.Template." + extension)))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public class ViewModel
    {
        public string Name { get; set; }

        public bool IsLoggedIn { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }

    public class TestHost : SuperSimpleViewEngine.IViewEngineHost
    {
        public string AntiForgeryToken()
        {
            return "";
        }

        public object Context
        {
            get { return this; }
        }

        public string ExpandPath(string path)
        {
            throw new NotImplementedException();
        }

        public string GetTemplate(string templateName, object model)
        {
            throw new NotImplementedException();
        }

        public string GetUriString(string name, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public string HtmlEncode(string input)
        {
            return HttpEncoder.HtmlEncode(input);
        }
    }
}