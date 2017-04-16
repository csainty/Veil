using System.Text.RegularExpressions;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleNameModelParser
    {
        private static Regex NameMatcher = new Regex(@".*?\[\'(?<Name>.*?)\'(,(?<Model>.*))?\]", RegexOptions.Compiled);

        public static SuperSimpleNameModel Parse(string expression)
        {
            var match = NameMatcher.Match(expression);
            var model = new SuperSimpleNameModel
            {
                Name = match.Groups["Name"].Value,
                Model = match.Groups["Model"].Value
            };

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new VeilParserException(string.Format("Failed to extract a name from '{0}'", expression));
            }

            return model;
        }
    }

    internal class SuperSimpleNameModel
    {
        public string Name { get; set; }

        public string Model { get; set; }
    }
}