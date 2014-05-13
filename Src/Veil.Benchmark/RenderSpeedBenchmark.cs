using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using RazorEngine;
using SimpleSpeedTester.Core;
using Veil.Benchmark.Hosts;

namespace Veil.Benchmark
{
    public class RenderSpeedBenchmark
    {
        private const int Test_Runs = 10000;

        public void Run()
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

            var handlebarsTemplate = Templates.ReadTemplate("Template.haml");
            var razorTemplate = Templates.ReadTemplate("Template.cshtml");
            var ssTemplate = Templates.ReadTemplate("Template.sshtml");

            var veilContext = new VeilContext();
            var veilEngine = new VeilEngine(veilContext);
            var superSimpleHost = new SuperSimpleHost();

            veilContext.RegisterTemplate("Master", Templates.ReadTemplate("Master.sshtml"));
            veilContext.RegisterTemplate("Roles", Templates.ReadTemplate("Roles.sshtml"));
            superSimpleHost.RegisterTemplate("Master", Templates.ReadTemplate("Master.sshtml"));
            superSimpleHost.RegisterTemplate("Roles", Templates.ReadTemplate("Roles.sshtml"));

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
        }

        private static void Execute(string name, Func<string> sample)
        {
            Console.WriteLine("Executing " + name);
            Templates.AssertTemplateSample(sample(), name);
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
    }
}