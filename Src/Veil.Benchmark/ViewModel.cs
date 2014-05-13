using System.Collections.Generic;

namespace Veil.Benchmark
{
    internal class ViewModel
    {
        public string Name { get; set; }

        public bool IsLoggedIn { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}