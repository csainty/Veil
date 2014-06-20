using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    /// <summary>
    /// Veil parser for the Handlebars syntax
    /// </summary>
    public class HandlebarsParser : ITemplateParser
    {
        private const string OverrideSectionName = "body";

        private static readonly Dictionary<Func<string, bool>, Action<HandlebarsParserState>> SyntaxHandlers = new Dictionary<Func<string, bool>, Action<HandlebarsParserState>>
        {
            { x => true, HandleStringLiteral },
            { x => true, HandleHtmlEscape },
            { x => x.StartsWith("~"), HandleTrimLastLiteral },
            { x => x.EndsWith("~"), HandleTrimNextLiteral },
            { x => x.StartsWith("!"), x => { /* Ignore Comments */ }},
            { x => x.StartsWith("#if"), HandleIf },
            { x => x == "else", HandleElse },
            { x => x == "/if", HandleEndIf },
            { x => x.StartsWith("#unless"), HandleUnless },
            { x => x == "/unless", HandleEndUnless },
            { x => x.StartsWith("#each"), HandleEach },
            { x => x == "/each", HandleEndEach },
            { x => x == "#flush", HandleFlush },
            { x => x.StartsWith("#with"), HandleWith },
            { x => x == "/with", HandleEndWith },
            { x => x.StartsWith(">"), HandlePartial },
            { x => x.StartsWith("<"), HandleMaster },
            { x => x == "body", HandleBody },
            { x => true, HandleExpression }
        };

        public SyntaxTreeNode Parse(TextReader templateReader, Type modelType)
        {
            var state = new HandlebarsParserState();
            var tokens = HandlebarsTokenizer.Tokenize(templateReader);
            state.BlockStack.PushNewBlockWithModel(modelType);

            foreach (var token in tokens)
            {
                state.SetCurrentToken(token);

                foreach (var handler in SyntaxHandlers)
                {
                    if (handler.Key(state.TokenText))
                    {
                        handler.Value.Invoke(state);
                        if (state.ContinueProcessingToken)
                        {
                            state.ContinueProcessingToken = false;
                            continue;
                        }
                        break;
                    }
                }
            }

            state.AssertStackOnRootNode();
            state.AssertPrefixesAreEmpty();

            return state.RootNode;
        }

        private static void HandleStringLiteral(HandlebarsParserState state)
        {
            if (state.CurrentToken.IsSyntaxToken)
            {
                state.ContinueProcessingToken = true;
                return;
            }

            state.WriteLiteral(state.CurrentToken.Content);
        }

        private static void HandleTrimLastLiteral(HandlebarsParserState state)
        {
            var literal = state.LastNode() as WriteLiteralNode;
            if (literal != null)
            {
                literal.LiteralContent = literal.LiteralContent.TrimEnd();
            }
            state.TokenText = state.TokenText.TrimStart('~', ' ', '\t');
            state.ContinueProcessingToken = true;
        }

        private static void HandleTrimNextLiteral(HandlebarsParserState state)
        {
            state.TrimNextLiteral = true;
            state.TokenText = state.TokenText.TrimEnd('~', ' ', '\t');
            state.ContinueProcessingToken = true;
        }

        private static void HandleHtmlEscape(HandlebarsParserState state)
        {
            state.HtmlEscapeCurrentExpression = state.CurrentToken.Content.Count(c => c == '{') == 2;
            state.ContinueProcessingToken = true;
        }

        private static void HandleIf(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block();
            var conditional = SyntaxTree.Conditional(state.ParseExpression(state.TokenText.Substring(4)), block);
            state.AddNodeToCurrentBlock(conditional);
            state.BlockStack.PushModelInheritingBlock(block);
        }

        private static void HandleElse(HandlebarsParserState state)
        {
            state.AssertInsideConditionalOrIteration("{{else}}");
            if (state.IsInEachBlock())
            {
                HandleIterationElse(state);
            }
            else
            {
                HandleConditionalElse(state);
            }
        }

        private static void HandleIterationElse(HandlebarsParserState state)
        {
            var elseBlock = state.BlockStack.GetCurrentBlockContainer<IterateNode>().EmptyBody;
            state.BlockStack.PopBlock();
            state.BlockStack.PushModelInheritingBlock(elseBlock);
        }

        private static void HandleConditionalElse(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block();
            state.BlockStack.GetCurrentBlockContainer<ConditionalNode>().FalseBlock = block;
            state.BlockStack.PopBlock();
            state.BlockStack.PushModelInheritingBlock(block);
        }

        private static void HandleEndIf(HandlebarsParserState state)
        {
            state.AssertInsideConditional("{{/if}}");
            state.BlockStack.PopBlock();
        }

        private static void HandleUnless(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block();
            var conditional = SyntaxTree.Conditional(state.ParseExpression(state.TokenText.Substring(8)), SyntaxTree.Block(), block);
            state.AddNodeToCurrentBlock(conditional);
            state.BlockStack.PushModelInheritingBlock(block);
        }

        private static void HandleEndUnless(HandlebarsParserState state)
        {
            state.AssertInsideConditional("{{/unless}}");
            state.BlockStack.PopBlock();
        }

        private static void HandleEach(HandlebarsParserState state)
        {
            var iteration = SyntaxTree.Iterate(
                state.ParseExpression(state.TokenText.Substring(6)),
                SyntaxTree.Block()
            );
            state.AddNodeToCurrentBlock(iteration);
            state.BlockStack.PushBlock(new HandlebarsParserBlock { Block = iteration.Body, ModelInScope = iteration.ItemType });
        }

        private static void HandleEndEach(HandlebarsParserState state)
        {
            state.AssertInsideIteration("{{/each}}");
            state.BlockStack.PopBlock();
        }

        private static void HandleFlush(HandlebarsParserState state)
        {
            state.AddNodeToCurrentBlock(SyntaxTree.Flush());
        }

        private static void HandleWith(HandlebarsParserState state)
        {
            state.ExpressionPrefixes.Push(state.TokenText.Substring(6).Trim());
        }

        private static void HandleEndWith(HandlebarsParserState state)
        {
            state.AssertHaveWithPrefix("{{/with}}");
            state.ExpressionPrefixes.Pop();
        }

        private static void HandlePartial(HandlebarsParserState state)
        {
            var partialTemplateName = state.TokenText.Substring(1).Trim();
            var self = Expression.Self(state.BlockStack.GetCurrentModelType());
            state.AddNodeToCurrentBlock(SyntaxTree.Include(partialTemplateName, self));
        }

        private static void HandleMaster(HandlebarsParserState state)
        {
            state.AssertSyntaxTreeIsEmpty("There can be no content before a {{< }} expression.");
            var masterTemplateName = state.TokenText.Substring(1).Trim();
            state.ExtendNode = SyntaxTree.Extend(masterTemplateName, new Dictionary<string, SyntaxTreeNode>
            {
                { OverrideSectionName, state.BlockStack.GetCurrentBlockNode() }
            });
        }

        private static void HandleBody(HandlebarsParserState state)
        {
            state.AddNodeToCurrentBlock(SyntaxTree.Override(OverrideSectionName));
        }

        private static void HandleExpression(HandlebarsParserState state)
        {
            var expression = state.ParseExpression(state.TokenText);
            state.AddNodeToCurrentBlock(SyntaxTree.WriteExpression(expression, state.HtmlEscapeCurrentExpression));
        }
    }
}