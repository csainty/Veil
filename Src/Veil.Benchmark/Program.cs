using System;
using Veil.Handlebars;
using Veil.SuperSimple;

namespace Veil.Benchmark
{
    public class Program
    {
        private static void Main(string[] args)
        {
            bool isManual = args.Length == 0;
            string option = "";

            Console.WriteLine("Veil.Benchmarks");
            Console.WriteLine("---------------");

            if (isManual)
            {
                Console.WriteLine("1. Render speed");
                Console.WriteLine("2. GC pressure");

                var key = Console.ReadKey(true);
                Console.Clear();
                option = key.KeyChar.ToString();
            }
            else
            {
                option = args[0];
            }

            if (option == "1") new RenderSpeedBenchmark().Run();
            if (option == "2") new GCBenchmark().Run();

            Console.WriteLine();
            Console.WriteLine("-- Done --");
            if (isManual) Console.ReadKey();
        }

        public static bool IsNix
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }
}