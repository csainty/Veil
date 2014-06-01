using System;
using System.Collections.Generic;
using System.Linq;
using SimpleSpeedTester.Core;
using Veil.Benchmark.RenderSpeedBenchmarkCases;

namespace Veil.Benchmark
{
    public class RenderSpeedBenchmark
    {
        private const int Test_Runs = 1000;

        public void Run()
        {
            var typedModel = ViewModels.CreateTypedViewModel();
            var dynamicModel = ViewModels.CreateDynamicViewModel();
            var dictionaryModel = ViewModels.CreateDictionaryModel();
            var benchmarks = GetBenchmarkCases().ToArray();

            Console.WriteLine("Warming up and validating benchmarks");

            foreach (var benchmark in benchmarks)
            {
                Templates.AssertTemplateSample(benchmark.RenderTypedModel(typedModel), benchmark.Name + " (Typed Model)");
                if (benchmark.SupportsUntypedModel) Templates.AssertTemplateSample(benchmark.RenderUntypedModel(typedModel), benchmark.Name + " (Untyped Model)");
                if (benchmark.SupportsDynamicModel) Templates.AssertTemplateSample(benchmark.RenderDynamicModel(dynamicModel), benchmark.Name + " (Dynamic Model)");
                if (benchmark.SupportsDictionaryModel) Templates.AssertTemplateSample(benchmark.RenderDictionaryModel(dictionaryModel), benchmark.Name + " (Dictionary Model)");
            }

            Console.WriteLine("----------");

            foreach (var benchmark in benchmarks)
            {
                Execute(() => benchmark.RenderTypedModel(typedModel), benchmark.Name + " (Typed Model)");
                if (benchmark.SupportsUntypedModel) Execute(() => benchmark.RenderUntypedModel(typedModel), benchmark.Name + " (Untyped Model)");
                if (benchmark.SupportsDynamicModel) Execute(() => benchmark.RenderDynamicModel(dynamicModel), benchmark.Name + " (Dynamic Model)");
                if (benchmark.SupportsDictionaryModel) Execute(() => benchmark.RenderDictionaryModel(dictionaryModel), benchmark.Name + " (Dictionary Model)");
            }

            foreach (var benchmark in benchmarks.OfType<IDisposable>())
            {
                benchmark.Dispose();
            }
        }

        private IEnumerable<IRenderSpeedBenchmarkCase> GetBenchmarkCases()
        {
            yield return new VeilSuperSimpleRenderSpeedBenchmarkCase();
            yield return new SuperSimpleRenderSpeedBenchmarkCase();
            yield return new VeilHandlebarsRenderSpeedBenchmarkCase();
            if (!Program.IsNix) yield return new ChevronRenderSpeedBenchmarkCase();
            yield return new RazorRenderSpeedBenchmarkCase();
        }

        private static void Execute(Func<string> sample, string name)
        {
            Console.WriteLine("Executing " + name);
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