using System;
using System.Diagnostics;
using Veil.Benchmark.GCBenchmarkCases;

namespace Veil.Benchmark
{
    public class GCBenchmark
    {
        private const int TestCount = 10000;
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
            var benchmarks = new IGCBenchmarkCase[] {
                new VeilGCBenchmarkCase(),
                new SuperSimpleGCBenchmarkCase()
            };

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
        }

        private void Execute(IGCBenchmarkCase benchmark, ViewModel model)
        {
            Console.WriteLine("Executing " + benchmark.Name);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var gen0Count = gen0Counter.RawValue;
            var gen1Count = gen1Counter.RawValue;
            var gen2Count = gen2Counter.RawValue;

            for (var i = 0; i < TestCount; i++)
            {
                benchmark.Render(NullTextWriter.Instance, model);
            }

            var gen0Done = gen0Counter.RawValue - gen0Count;
            var gen1Done = gen1Counter.RawValue - gen1Count;
            var gen2Done = gen2Counter.RawValue - gen2Count;

            Console.WriteLine(gen0Done + "/" + gen1Done + "/" + gen2Done);
        }
    }
}