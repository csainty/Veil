using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil
{
    /// <summary>
    /// Configures Veil with syntax parsers
    /// </summary>
    public static class VeilStaticConfiguration
    {
        private static IDictionary<string, Func<ITemplateParser>> parserFactories = new Dictionary<string, Func<ITemplateParser>>();

        static VeilStaticConfiguration()
        {
            ScanForParsersInAppDomain();
        }

        private static void ScanForParsersInAppDomain()
        {
            foreach (var registration in AssemblyParserFinder.ParserRegistrations)
            {
                foreach (var key in registration.Keys)
                {
                    RegisterParser(key, registration.ParserFactory);
                }
            }
        }

        /// <summary>
        /// Registers a parser instance for use by the engine
        /// </summary>
        /// <param name="templateType">The key that will be used to signal the engine to use this parser. See <see cref="VeilEngine.Compile"/></param>
        /// <param name="parser">An instance of the parser that will be reused for each compile</param>
        public static void RegisterParser(string templateType, ITemplateParser parser)
        {
            RegisterParser(templateType, () => parser);
        }

        /// <summary>
        /// Registers a parser factory for use by the engine
        /// </summary>
        /// <param name="templateType">The key that will be used to signal the engine to use this parser. See <see cref="VeilEngine.Compile"/></param>
        /// <param name="parserFactory">A factory for the parser. The factory is invoked once for each compile</param>
        public static void RegisterParser(string templateType, Func<ITemplateParser> parserFactory)
        {
            if (String.IsNullOrEmpty(templateType)) throw new ArgumentNullException("templateType");
            if (parserFactories.ContainsKey(templateType)) throw new ArgumentException("A parser for templateType '{0}' ({1}) is already registered.".FormatInvariant(templateType, parserFactories[templateType].GetType().Name), "templateType");

            parserFactories.Add(templateType, parserFactory);
        }

        /// <summary>
        /// Clear all currently registered parsers
        /// </summary>
        public static void ClearParserRegistrations()
        {
            parserFactories.Clear();
        }

        /// <summary>
        /// Determines whether a parser with the specified key is already registered
        /// </summary>
        public static bool IsParserRegistered(string key)
        {
            return parserFactories.ContainsKey(key);
        }

        /// <summary>
        /// Returns a parser instance for the specified key
        /// </summary>
        public static ITemplateParser GetParserInstance(string key)
        {
            return parserFactories[key].Invoke();
        }

        /// <summary>
        /// Gets a list of all currently registered parsers
        /// </summary>
        public static IEnumerable<string> RegisteredParserKeys
        {
            get { return parserFactories.Keys; }
        }
    }
}