using System.Collections.Generic;

namespace Veil.Benchmark.RenderSpeedBenchmarkCases
{
    public interface IRenderSpeedBenchmarkCase
    {
        string Name { get; }

        string RenderTypedModel(ViewModel model);

        string RenderUntypedModel(object model);

        string RenderDynamicModel(dynamic model);

        string RenderDictionaryModel(IDictionary<string, object> model);

        bool SupportsDictionaryModel { get; }

        bool SupportsDynamicModel { get; }

        bool SupportsUntypedModel { get; }
    }
}