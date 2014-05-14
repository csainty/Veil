using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Veil.Benchmark
{
    public static class Templates
    {
        public static void AssertTemplateSample(string sample, string engine)
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

        public static string ReadTemplate(string templateName)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Veil.Benchmark.Views." + templateName)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}