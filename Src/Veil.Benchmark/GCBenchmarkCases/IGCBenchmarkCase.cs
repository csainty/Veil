using System.IO;

namespace Veil.Benchmark.GCBenchmarkCases
{
    public interface IGCBenchmarkCase
    {
        string Name { get; }

        void Render(TextWriter writer, ViewModel model);
    }
}