using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleMasterPageParser
    {
        public static SyntaxTreeNode Parse(IEnumerable<SuperSimpleToken> tokens, Type modelType)
        {
            var masterName = "";
            var sections = new Dictionary<string, SyntaxTreeNode>();
            var currentSectionTokens = new List<SuperSimpleToken>();
            var sectionName = "";
            var inSection = false;

            foreach (var token in tokens)
            {
                var currentToken = token.Content.Trim(new[] { '@', ';' });
                if (currentToken.StartsWith("Master["))
                {
                    masterName = SuperSimpleNameModelParser.Parse(currentToken).Name;
                }
                else if (currentToken.StartsWith("Section[") && !inSection)
                {
                    sectionName = SuperSimpleNameModelParser.Parse(currentToken).Name;
                    inSection = true;
                }
                else if (currentToken == "EndSection")
                {
                    var block = SuperSimpleTemplateParser.Parse(currentSectionTokens, modelType);
                    sections.Add(sectionName, block);
                    inSection = false;
                    currentSectionTokens.Clear();
                }
                else if (inSection)
                {
                    currentSectionTokens.Add(token);
                }
            }
            return SyntaxTree.Extend(masterName, sections);
        }
    }
}