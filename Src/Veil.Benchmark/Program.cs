using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
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
        private const int Test_Runs = 10000;

        private static void Main(string[] args)
        {
            var model = new ViewModel
            {
                Name = "Test Template",
                IsLoggedIn = true,
                Roles = new[] { "User", "Admin", "Editor", "Viewer", "Uploader", "Pick & Pack" }
            };

            dynamic dynamicModel = new ExpandoObject();
            dynamicModel.Name = "Test Template";
            dynamicModel.IsLoggedIn = true;
            dynamicModel.Roles = new[] { "User", "Admin", "Editor", "Viewer", "Uploader", "Pick & Pack" };

            var dictionaryModel = new Dictionary<string, object>();
            dictionaryModel.Add("Name", "Test Template");
            dictionaryModel.Add("IsLoggedIn", true);
            dictionaryModel.Add("Roles", new[] { "User", "Admin", "Editor", "Viewer", "Uploader", "Pick & Pack" });

            var handlebarsTemplate = ReadTemplate("Template.haml");
            var razorTemplate = ReadTemplate("Template.cshtml");
            var ssTemplate = ReadTemplate("Template.sshtml");

            VeilEngine.RegisterParser("haml", new HandlebarsParser());
            VeilEngine.RegisterParser("sshtml", new SuperSimpleParser());
            var veilContext = new BenchmarkVeilContext();
            var veilEngine = new VeilEngine(veilContext);
            var superSimpleHost = new BenchmarkHost();

            veilContext.RegisterTemplate("Master", ReadTemplate("Master.sshtml"));
            veilContext.RegisterTemplate("Roles", ReadTemplate("Roles.sshtml"));
            superSimpleHost.RegisterTemplate("Master", ReadTemplate("Master.sshtml"));
            superSimpleHost.RegisterTemplate("Roles", ReadTemplate("Roles.sshtml"));

            {
                var template = veilEngine.Compile<ViewModel>("haml", new StringReader(handlebarsTemplate));
                Execute("Veil.Handlebars", () =>
                {
                    using (var writer = new StringWriter())
                    {
                        template(writer, model);
                        return writer.ToString();
                    }
                });
            }

            using (var handlebars = new Chevron.Handlebars())
            {
                handlebars.RegisterTemplate("default", handlebarsTemplate);
                Execute("Chevron.IE.Merged", () =>
                {
                    return handlebars.Transform("default", model);
                });
            }

            {
                Razor.Compile<ViewModel>(razorTemplate, "Test");
                Execute("Razor", () =>
                {
                    return Razor.Run<ViewModel>("Test", model);
                });
            }

            {
                var template = veilEngine.Compile<ViewModel>("sshtml", new StringReader(ssTemplate));
                Execute("Veil.SuperSimple", () =>
                {
                    using (var writer = new StringWriter())
                    {
                        template(writer, model);
                        return writer.ToString();
                    }
                });
            }

            {
                var engine = new SuperSimpleViewEngine.SuperSimpleViewEngine();
                Execute("SuperSimpleViewEngine", () =>
                {
                    return engine.Render(ssTemplate, model, superSimpleHost);
                });
            }

            {
                var template = veilEngine.CompileNonGeneric("sshtml", new StringReader(ssTemplate), typeof(ViewModel));
                Execute("Veil.NonGeneric.SuperSimple", () =>
                {
                    using (var writer = new StringWriter())
                    {
                        template(writer, model);
                        return writer.ToString();
                    }
                });
            }

            {
                var template = veilEngine.Compile<dynamic>("sshtml", new StringReader(ssTemplate));
                Execute("Veil.DynamicModel", () =>
                {
                    using (var writer = new StringWriter())
                    {
                        template(writer, dynamicModel);
                        return writer.ToString();
                    }
                });
            }

            {
                var template = veilEngine.Compile<IDictionary<string, object>>("sshtml", new StringReader(ssTemplate));
                Execute("Veil.DictionaryModel", () =>
                {
                    using (var writer = new StringWriter())
                    {
                        template(writer, dictionaryModel);
                        return writer.ToString();
                    }
                });
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void Execute(string name, Func<string> sample)
        {
            Console.WriteLine("Executing " + name);
            AssertTemplateSample(sample(), name);
            var testGroup = new TestGroup(name).Plan("Execute", () => sample(), Test_Runs).GetResult();
            Console.WriteLine("Total: {0}ms (" + Test_Runs + " runs)", testGroup.Outcomes.Select(x => x.Elapsed.TotalMilliseconds).Sum());
            Console.WriteLine("Avg  : {0}ms", testGroup.Outcomes.Select(x => x.Elapsed.TotalMilliseconds).Average());
            Console.WriteLine("Min  : {0}ms", testGroup.Outcomes.Select(x => x.Elapsed.TotalMilliseconds).Min());
            Console.WriteLine("Max  : {0}ms", testGroup.Outcomes.Select(x => x.Elapsed.TotalMilliseconds).Max());
            if (testGroup.Outcomes.Any(x => x.Exception != null))
            {
                Console.WriteLine("!!! -- Exception thrown by one or more test samples -- !!!");
            }
            Console.WriteLine("------------------------------------------");
        }

        private static void AssertTemplateSample(string sample, string engine)
        {
            string expectedResult = ReadTemplate("Template.txt");
            expectedResult = Regex.Replace(expectedResult, @"\s", "");
            sample = Regex.Replace(sample, @"\s", "");
            if (!String.Equals(expectedResult, sample))
            {
                Console.WriteLine("!!! -- Sample didn't match for test " + engine + " -- !!!");
                Console.WriteLine(sample.Replace("\r\n", " "));
            }
        }

        private static string ReadTemplate(string templateName)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Veil.Benchmark." + templateName)))
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

    public class BenchmarkHost : SuperSimpleViewEngine.IViewEngineHost
    {
        private Dictionary<string, string> registeredTemplates = new Dictionary<string, string>();

        public void RegisterTemplate(string name, string content)
        {
            registeredTemplates.Add(name, content);
        }

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
            return registeredTemplates[templateName];
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

    public class BenchmarkVeilContext : IVeilContext
    {
        private Dictionary<string, string> registeredTemplates = new Dictionary<string, string>();

        public void RegisterTemplate(string name, string content)
        {
            registeredTemplates.Add(name, content);
        }

        public TextReader GetTemplateByName(string name, string templateType)
        {
            return new StringReader(registeredTemplates[name]);
        }
    }
}