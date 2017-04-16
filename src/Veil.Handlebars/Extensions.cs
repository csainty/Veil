using System;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    internal static class Extensions
    {
        public static void AssertStackOnRootNode(this HandlebarsParserState state)
        {
            if (state.BlockStack.Count != 1)
            {
                throw new VeilParserException(String.Format("Mismatched block found. Expected to find the end of the template but found '{0}' open blocks.", state.BlockStack.Count));
            }
        }

        public static void AssertInsideConditional(this HandlebarsParserState state, string foundToken)
        {
            var faulted = state.BlockStack.Count < 2;
            faulted = faulted || !state.BlockStack.IsCurrentBlockContainerOfType<ConditionalNode>();

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a conditional block.", foundToken));
            }
        }

        public static void AssertInsideIteration(this HandlebarsParserState state, string foundToken)
        {
            var faulted = state.BlockStack.Count < 2;
            faulted = faulted || !state.BlockStack.IsCurrentBlockContainerOfType<IterateNode>();

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of an iteration block.", foundToken));
            }
        }

        public static void AssertInsideWith(this HandlebarsParserState state, string foundToken)
        {
            var faulted = state.BlockStack.Count < 2;
            faulted = faulted || !state.BlockStack.IsCurrentBlockContainerOfType<ScopedNode>();

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a with block.", foundToken));
            }
        }

        public static void AssertInsideConditionalOrIteration(this HandlebarsParserState state, string foundToken)
        {
            var faulted = state.BlockStack.Count < 2;
            faulted = faulted || !(state.BlockStack.IsCurrentBlockContainerOfType<ConditionalNode>() || state.BlockStack.IsCurrentBlockContainerOfType<IterateNode>());

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a conditional or iteration block.", foundToken));
            }
        }

        public static void AssertSyntaxTreeIsEmpty(this HandlebarsParserState state, string errorMessage)
        {
            if (state.BlockStack.Count > 1 || !state.BlockStack.GetCurrentBlockNode().IsEmpty())
            {
                throw new VeilParserException(errorMessage);
            }
        }

        public static bool IsInEachBlock(this HandlebarsParserState state)
        {
            if (state.BlockStack.Count < 2)
            {
                return false;
            }

            return state.BlockStack.IsCurrentBlockContainerOfType<IterateNode>();
        }
    }
}