using System;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    internal static class Extensions
    {
        public static void AssertStackOnRootNode(this HandlebarsParserState state)
        {
            if (state.Scopes.Count != 1)
            {
                throw new VeilParserException(String.Format("Mismatched block found. Expected to find the end of the template but found '{0}' open blocks.", state.Scopes.Count));
            }
        }

        public static void AssertPrefixesAreEmpty(this HandlebarsParserState state)
        {
            if (state.ExpressionPrefixes.Count > 0)
            {
                throw new VeilParserException(String.Format("Mismatched block found. Expected to find the end of the template but found open with block '{0}'", String.Join(".", state.ExpressionPrefixes)));
            }
        }

        public static void AssertInsideConditional(this HandlebarsParserState state, string foundToken)
        {
            var faulted = state.Scopes.Count < 2;
            faulted = faulted || !state.Scopes.IsCurrentScopeContainer<ConditionalNode>();

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a conditional block.", foundToken));
            }
        }

        public static void AssertInsideIteration(this HandlebarsParserState state, string foundToken)
        {
            var faulted = state.Scopes.Count < 2;
            faulted = faulted || !state.Scopes.IsCurrentScopeContainer<IterateNode>();

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of an iteration block.", foundToken));
            }
        }

        public static void AssertInsideConditionalOrIteration(this HandlebarsParserState state, string foundToken)
        {
            var faulted = state.Scopes.Count < 2;
            faulted = faulted || !(state.Scopes.IsCurrentScopeContainer<ConditionalNode>() || state.Scopes.IsCurrentScopeContainer<IterateNode>());

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a conditional or iteration block.", foundToken));
            }
        }

        public static void AssertSyntaxTreeIsEmpty(this HandlebarsParserState state, string errorMessage)
        {
            if (state.Scopes.Count > 1 || !state.Scopes.GetCurrentBlock().IsEmpty())
            {
                throw new VeilParserException(errorMessage);
            }
        }

        public static void AssertHaveWithPrefix(this HandlebarsParserState state, string foundToken)
        {
            if (state.ExpressionPrefixes.Count == 0)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a with block.", foundToken));
            }
        }

        public static bool IsInEachBlock(this HandlebarsParserState state)
        {
            if (state.Scopes.Count < 2)
            {
                return false;
            }

            return state.Scopes.IsCurrentScopeContainer<IterateNode>();
        }
    }
}