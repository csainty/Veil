using System;
using Veil.Handlebars;
using Veil.SuperSimple;

namespace Veil.Benchmark
{
    public class Program
    {
        private static void Main(string[] args)
        {
            VeilEngine.RegisterParser("haml", new HandlebarsParser());
            VeilEngine.RegisterParser("sshtml", new SuperSimpleParser());

            Console.WriteLine("Veil.Benchmarks");
            Console.WriteLine("---------------");
            Console.WriteLine("1. Render speed");

            var key = Console.ReadKey(true);
            Console.Clear();
            if (key.KeyChar == '1') new RenderSpeedBenchmark().Run();

            Console.WriteLine();
            Console.WriteLine("-- Done --");
            Console.ReadKey();
        }
    }
}