using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly Dictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>> SyntaxHandlers = new Dictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>>
        {
            { x => !x.IsSyntaxToken, HandleStringLiteral },
            { x => x.TrimLastLiteral, HandleTrimLastLiteral },
            { x => x.TrimNextLiteral, HandleTrimNextLiteral },
            { x => x.Content.StartsWith("!"), x => { /* Ignore Comments */ }},
            { x => x.Content.StartsWith("#if"), HandleIf },
            { x => x.Content == "else", HandleElse },
            { x => x.Content == "/if", HandleEndIf },
            { x => x.Content.StartsWith("#unless"), HandleUnless },
            { x => x.Content == "/unless", HandleEndUnless },
            { x => x.Content.StartsWith("#each"), HandleEach },
            { x => x.Content == "/each", HandleEndEach },
            { x => x.Content == "#flush", HandleFlush },
            { x => x.Content.StartsWith("#with"), HandleWith },
            { x => x.Content == "/with", HandleEndWith },
            { x => x.Content.StartsWith(">"), HandlePartial },
            { x => x.Content.StartsWith("<"), HandleMaster },
            { x => x.Content == "body", HandleBody },
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
                    if (handler.Key(token))
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

            return state.RootNode;
        }

        private static void HandleStringLiteral(HandlebarsParserState state)
        {
            state.WriteLiteral(state.CurrentToken.Content);
        }

        private static void HandleTrimLastLiteral(HandlebarsParserState state)
        {
            var literal = state.LastNode() as WriteLiteralNode;
            if (literal != null)
            {
                literal.LiteralContent = literal.LiteralContent.TrimEnd();
            }
            state.ContinueProcessingToken = true;
        }

        private static void HandleTrimNextLiteral(HandlebarsParserState state)
        {
            state.TrimNextLiteral = true;
            state.ContinueProcessingToken = true;
        }

        private static void HandleIf(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block();
            var conditional = SyntaxTree.Conditional(state.ParseExpression(state.CurrentToken.Content.Substring(4)), block);
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
            var conditional = SyntaxTree.Conditional(state.ParseExpression(state.CurrentToken.Content.Substring(8)), SyntaxTree.Block(), block);
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
                state.ParseExpression(state.CurrentToken.Content.Substring(6)),
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
            var withBlock = SyntaxTree.Block();
            var modelExpression = state.ParseExpression(state.CurrentToken.Content.Substring(6).Trim());
            state.AddNodeToCurrentBlock(SyntaxTree.ScopeNode(modelExpression, withBlock));
            state.BlockStack.PushBlock(new HandlebarsParserBlock
            {
                Block = withBlock,
                ModelInScope = modelExpression.ResultType
            });
        }

        private static void HandleEndWith(HandlebarsParserState state)
        {
            state.AssertInsideWith("{{/with}}");
            state.BlockStack.PopBlock();
        }

        private static void HandlePartial(HandlebarsParserState state)
        {
            var partialTemplateName = state.CurrentToken.Content.Substring(1).Trim();
            var self = SyntaxTreeExpression.Self(state.BlockStack.GetCurrentModelType());
            state.AddNodeToCurrentBlock(SyntaxTree.Include(partialTemplateName, self));
        }

        private static void HandleMaster(HandlebarsParserState state)
        {
            state.AssertSyntaxTreeIsEmpty("There can be no content before a {{< }} expression.");
            var masterTemplateName = state.CurrentToken.Content.Substring(1).Trim();
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
            var expression = state.ParseExpression(state.CurrentToken.Content);
            state.AddNodeToCurrentBlock(SyntaxTree.WriteExpression(expression, state.CurrentToken.IsHtmlEscape));
        }
    }
}