using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Veil.Benchmark.GCBenchmarkCases;

namespace Veil.Benchmark
{
    public class GCBenchmark
    {
        private const int TestCount = 100000;
        private readonly PerformanceCounter gen0Counter;
        private readonly PerformanceCounter gen1Counter;
        private readonly PerformanceCounter gen2Counter;

        public GCBenchmark()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            gen0Counter = new PerformanceCounter(".NET CLR Memory", "# Gen 0 Collections", processName);
            gen1Counter = new PerformanceCounter(".NET CLR Memory", "# Gen 1 Collections", processName);
            gen2Counter = new PerformanceCounter(".NET CLR Memory", "# Gen 2 Collections", processName);
        }

        public void Run()
        {
            var model = ViewModels.CreateTypedViewModel();
            var benchmarks = GetBenchmarkCases().ToArray();

            Console.WriteLine("Warming up benchmarks");
            for (int i = 0, j = benchmarks.Length; i < j; i++)
            {
                benchmarks[i].Render(NullTextWriter.Instance, model);
            }
            Console.WriteLine("-----------");

            for (int i = 0, j = benchmarks.Length; i < j; i++)
            {
                Execute(benchmarks[i], model);
            }

            foreach (var benchmark in benchmarks.OfType<IDisposable>())
            {
                benchmark.Dispose();
            }
        }

        private IEnumerable<IGCBenchmarkCase> GetBenchmarkCases()
        {
            yield return new VeilGCBenchmarkCase();
            yield return new SuperSimpleGCBenchmarkCase();
            if (!Program.IsNix) yield return new ChevronGCBenchmarkCase();
            yield return new RazorGCBenchmarkCase();
        }

        private void Execute(IGCBenchmarkCase benchmark, ViewModel model)
        {
            Console.WriteLine(String.Format("Executing {0} ({1} runs)", benchmark.Name, TestCount));
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var gen0Count = gen0Counter.RawValue;
            var gen1Count = gen1Counter.RawValue;
            var gen2Count = gen2Counter.RawValue;

            for (var i = 0; i < TestCount; i++)
            {
                benchmark.Render(NullTextWriter.Instance, model);
            }

            Console.WriteLine(String.Format("GC Gen Collections - {0}/{1}/{2}",
                gen0Counter.RawValue - gen0Count,
                gen1Counter.RawValue - gen1Count,
                gen2Counter.RawValue - gen2Count
            ));
        }
    }
}