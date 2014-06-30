using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleMasterPageParser
    {
        public static SyntaxTreeNode Parse(IEnumerable<SuperSimpleToken> tokens, Type modelType)
        {
            var state = new SuperSimpleMasterPageParserState(modelType);

            foreach (var token in tokens)
            {
                if (token.Content.StartsWith("Master"))
                {
                    state.MasterPageName = SuperSimpleNameModelParser.Parse(token.Content).Name;
                }
                else if (token.Content.StartsWith("Section") && !state.IsProcessingASection)
                {
                    state.StartSection(SuperSimpleNameModelParser.Parse(token.Content).Name);
                }
                else if (token.Content == "EndSection")
                {
                    state.FinalizeCurrentSection();
                }
                else if (state.IsProcessingASection)
                {
                    state.AddTokenToCurrentSection(token);
                }
                else if (token.IsSyntaxToken)
                {
                    throw new VeilParserException(String.Format("Found expression '{0}' outside of a @Section block", token.Content));
                }
                else if (!String.IsNullOrWhiteSpace(token.Content))
                {
                    throw new VeilParserException(String.Format("Found content '{0}' outside of a @Section block", token.Content));
                }
            }
            return SyntaxTree.Extend(state.MasterPageName, state.Sections);
        }
    }
}