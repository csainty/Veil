using System;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    internal static class Extensions
    {
        public static void AssertStackOnRootNode(this HandlebarsScopeStack scopes)
        {
            if (scopes.Count != 1)
            {
                throw new VeilParserException(String.Format("Mismatched block found. Expected to find the end of the template but found '{0}' open blocks.", scopes.Count));
            }
        }

        public static void AssertInsideConditionalOnModelBlock(this HandlebarsScopeStack scopes, string foundToken)
        {
            var faulted = false;
            faulted = scopes.Count < 2;

            if (!faulted)
            {
                faulted = !(scopes.GetCurrentScopeContainer<SyntaxTreeNode>() is ConditionalNode);
            }

            if (faulted)
            {
                throw new VeilParserException(String.Format("Found token '{0}' outside of a conditional block.", foundToken));
            }
        }

        public static void AssertSyntaxTreeIsEmpty(this HandlebarsScopeStack scopes)
        {
            if (scopes.Count > 1 || !scopes.GetCurrentBlock().IsEmpty())
            {
                throw new VeilParserException("There can be no content before a {{< }} expression.");
            }
        }

        public static bool IsInEachBlock(this HandlebarsScopeStack scopes)
        {
            if (scopes.Count < 2)
            {
                return false;
            }

            return scopes.GetCurrentScopeContainer<SyntaxTreeNode>() is IterateNode;
        }
    }
}