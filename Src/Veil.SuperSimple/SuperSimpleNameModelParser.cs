﻿using System.Text.RegularExpressions;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleNameModelParser
    {
        private static Regex NameMatcher = new Regex(@".*?\[\'(?<Name>.*?)\'(,(?<Model>.*))?\]", RegexOptions.Compiled);

        public static SuperSimpleNameModel Parse(string expression)
        {
            var match = NameMatcher.Match(expression);
            return new SuperSimpleNameModel
            {
                Name = match.Groups["Name"].Value,
                Model = match.Groups["Model"].Value
            };
        }
    }

    internal class SuperSimpleNameModel
    {
        public string Name { get; set; }

        public string Model { get; set; }
    }
}