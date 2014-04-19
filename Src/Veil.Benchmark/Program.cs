using System;
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
            var model = new ViewModel { Name = "Test Template", IsLoggedIn = true };
            var handlebarsTemplate = ReadTemplate("haml");
            var razorTemplate = ReadTemplate("cshtml");
            var ssTemplate = ReadTemplate("sshtml");

            VeilEngine.RegisterParser("haml", new HandlebarsParser());
            VeilEngine.RegisterParser("sshtml", new SuperSimpleParser());
            var veilEngine = new VeilEngine();

            {
                var veilTemplate = veilEngine.Compile<ViewModel>("haml", new StringReader(handlebarsTemplate));
                AssertTemplateSample(Unwrap(veilTemplate, model));
                var veilGroup = new TestGroup("Veil.Handlebars").PlanAndExecute("Template", () =>
                {
                    veilTemplate(new StringWriter(), model);
                }, 5000);
                Console.WriteLine(veilGroup);
            }

            using (var handlebars = new Chevron.Handlebars())
            {
                handlebars.RegisterTemplate("default", handlebarsTemplate);
                AssertTemplateSample(handlebars.Transform("default", model));
                var chevronGroup = new TestGroup("Chevron.IE.Merged").PlanAndExecute("Template", () =>
                {
                    handlebars.Transform("default", model);
                }, 5000);
                Console.WriteLine(chevronGroup);
            }

            {
                Razor.Compile<ViewModel>(razorTemplate, "Test");
                AssertTemplateSample(Razor.Run<ViewModel>("Test", model));
                var razorGroup = new TestGroup("Razor").PlanAndExecute("Template", () =>
                {
                    Razor.Run<ViewModel>("Test", model);
                }, 5000);
                Console.WriteLine(razorGroup);
            }

            {
                var veilTemplate = veilEngine.Compile<ViewModel>("sshtml", new StringReader(ssTemplate));
                AssertTemplateSample(Unwrap(veilTemplate, model));
                var ssGroup = new TestGroup("Veil.SuperSimple").PlanAndExecute("Template", () =>
                {
                    veilTemplate(new StringWriter(), model);
                }, 5000);
                Console.WriteLine(ssGroup);
            }

            {
                var engine = new SuperSimpleViewEngine.SuperSimpleViewEngine();
                var host = new TestHost();
                AssertTemplateSample(engine.Render(ssTemplate, model, host));
                var ssGroup = new TestGroup("SuperSimpleViewEngine").PlanAndExecute("Template", () =>
                {
                    engine.Render(ssTemplate, model, host);
                }, 5000);
                Console.WriteLine(ssGroup);
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

        private static void AssertTemplateSample(string sample)
        {
            string expectedResult = ReadTemplate("txt");
            expectedResult = Regex.Replace(expectedResult, @"\s", "");
            sample = Regex.Replace(sample, @"\s", "");
            if (!String.Equals(expectedResult, sample))
            {
                throw new InvalidOperationException("Sample didn't match");
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
            throw new NotImplementedException();
        }
    }
}