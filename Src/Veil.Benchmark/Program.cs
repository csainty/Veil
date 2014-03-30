using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Chevron;
using SimpleSpeedTester.Core;

namespace Veil.Benchmark
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var model = new ViewModel { Name = "Test Template", IsLoggedIn = true };
            string template = "";
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Veil.Benchmark.Template.haml")))
            {
                template = reader.ReadToEnd();
            }

            {
                Console.WriteLine("Testing Veil.Hail...");
                var veilEngine = new VeilEngine();
                var veilTemplate = veilEngine.Compile<ViewModel>(new StringReader(template));
                AssertTemplateSample(Unwrap(veilTemplate, model));
                var veilGroup = new TestGroup("Veil.Hail").PlanAndExecute("Template", () =>
                {
                    veilTemplate(new StringWriter(), model);
                }, 5000);
                Console.WriteLine(veilGroup);
            }

            using (var handlebars = new Handlebars())
            {
                Console.WriteLine("Testing Chevron.IE.Merged...");
                handlebars.RegisterTemplate("default", template);
                AssertTemplateSample(handlebars.Transform("default", model));
                var chevronGroup = new TestGroup("Chevron.IE.Merged").PlanAndExecute("Template", () =>
                {
                    handlebars.Transform("default", model);
                }, 5000);
                Console.WriteLine(chevronGroup);
            }

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
            string expectedResult;
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Veil.Benchmark.Template.txt")))
            {
                expectedResult = reader.ReadToEnd();
            }
            expectedResult = Regex.Replace(expectedResult, @"\s", "");
            sample = Regex.Replace(sample, @"\s", "");
            if (!String.Equals(expectedResult, sample))
            {
                throw new InvalidOperationException("Sample didn't match");
            }
        }
    }

    public class ViewModel
    {
        public string Name { get; set; }

        public bool IsLoggedIn { get; set; }
    }
}