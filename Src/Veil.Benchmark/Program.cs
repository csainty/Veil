using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using RazorEngine;
using SimpleSpeedTester.Core;
using Veil.Handlebars;

namespace Veil.Benchmark
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var model = new ViewModel { Name = "Test Template", IsLoggedIn = true };
            string handlebarsTemplate = ReadTemplate("haml");
            string razorTemplate = ReadTemplate("cshtml");

            VeilEngine.RegisterParser("handlebars", new HandlebarsParser());

            {
                Console.WriteLine("Testing Veil.Hail...");
                var veilEngine = new VeilEngine();
                var veilTemplate = veilEngine.Compile<ViewModel>("handlebars", new StringReader(handlebarsTemplate));
                AssertTemplateSample(Unwrap(veilTemplate, model));
                var veilGroup = new TestGroup("Veil.Hail").PlanAndExecute("Template", () =>
                {
                    veilTemplate(new StringWriter(), model);
                }, 5000);
                Console.WriteLine(veilGroup);
            }

            using (var handlebars = new Chevron.Handlebars())
            {
                Console.WriteLine("Testing Chevron.IE.Merged...");
                handlebars.RegisterTemplate("default", handlebarsTemplate);
                AssertTemplateSample(handlebars.Transform("default", model));
                var chevronGroup = new TestGroup("Chevron.IE.Merged").PlanAndExecute("Template", () =>
                {
                    handlebars.Transform("default", model);
                }, 5000);
                Console.WriteLine(chevronGroup);
            }

            {
                Console.WriteLine("Testing Razor...");
                Razor.Compile<ViewModel>(razorTemplate, "Test");
                AssertTemplateSample(Razor.Run<ViewModel>("Test", model));
                var razorGroup = new TestGroup("Chevron.IE.Merged").PlanAndExecute("Template", () =>
                {
                    Razor.Run<ViewModel>("Test", model);
                }, 5000);
                Console.WriteLine(razorGroup);
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
}