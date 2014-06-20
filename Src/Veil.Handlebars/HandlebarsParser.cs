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
        private static Dictionary<Func<string, bool>, Action<HandlebarsParserState>> SyntaxHandlers = new Dictionary<Func<string, bool>, Action<HandlebarsParserState>>
        {
            { x => x.StartsWith("!"), x => { /* Ignore Comments */ }},
            { x => true, HandleStringLiteral },
            { x => true, HandleHtmlEscape },
            { x => x.StartsWith("~"), HandleTrimLastLiteral },
            { x => x.EndsWith("~"), HandleTrimNextLiteral },
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
            state.Scopes.PushScope(modelType);

            foreach (var token in tokens)
            {
                state.SetCurrentToken(token);

                foreach (var handler in SyntaxHandlers)
                {
                    if (handler.Key(state.TokenText))
                    {
                        handler.Value(state);
                        if (state.ContinueProcessingToken)
                        {
                            state.ContinueProcessingToken = false;
                            continue;
                        }
                        break;
                    }
                }
            }

            state.Scopes.AssertStackOnRootNode();

            return state.ExtendNode ?? state.Scopes.GetCurrentBlock();
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
            var literal = state.Scopes.GetCurrentBlock().Nodes.Last() as WriteLiteralNode;
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
            state.HtmlEscape = state.CurrentToken.Content.Count(c => c == '{') == 2;
            state.ContinueProcessingToken = true;
        }

        private static void HandleIf(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block();
            var conditional = SyntaxTree.Conditional(state.ParseExpression(state.TokenText.Substring(4)), block);
            state.Scopes.AddToCurrentScope(conditional);
            state.Scopes.PushInheritedScope(block);
        }

        private static void HandleElse(HandlebarsParserState state)
        {
            if (state.Scopes.IsInEachBlock())
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
            var elseBlock = state.Scopes.GetCurrentScopeContainer<IterateNode>().EmptyBody;
            state.Scopes.PopScope();
            state.Scopes.PushInheritedScope(elseBlock);
        }

        private static void HandleConditionalElse(HandlebarsParserState state)
        {
            state.Scopes.AssertInsideConditionalOnModelBlock("{{else}}");
            var block = SyntaxTree.Block();
            state.Scopes.GetCurrentScopeContainer<ConditionalNode>().FalseBlock = block;
            state.Scopes.PopScope();
            state.Scopes.PushInheritedScope(block);
        }

        private static void HandleEndIf(HandlebarsParserState state)
        {
            state.Scopes.AssertInsideConditionalOnModelBlock("{{/if}}");
            state.Scopes.PopScope();
        }

        private static void HandleUnless(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block();
            var conditional = SyntaxTree.Conditional(state.ParseExpression(state.TokenText.Substring(8)), SyntaxTree.Block(), block);
            state.Scopes.AddToCurrentScope(conditional);
            state.Scopes.PushInheritedScope(block);
        }

        private static void HandleEndUnless(HandlebarsParserState state)
        {
            state.Scopes.AssertInsideConditionalOnModelBlock("{{/unless}}");
            state.Scopes.PopScope();
        }

        private static void HandleEach(HandlebarsParserState state)
        {
            var iteration = SyntaxTree.Iterate(
                state.ParseExpression(state.TokenText.Substring(6)),
                SyntaxTree.Block()
            );
            state.Scopes.AddToCurrentScope(iteration);
            state.Scopes.PushScope(new HandlebarsParserScope { Block = iteration.Body, ModelInScope = iteration.ItemType });
        }

        private static void HandleEndEach(HandlebarsParserState state)
        {
            state.Scopes.PopScope();
        }

        private static void HandleFlush(HandlebarsParserState state)
        {
            state.Scopes.AddToCurrentScope(SyntaxTree.Flush());
        }

        private static void HandleWith(HandlebarsParserState state)
        {
            state.PushExpressionPrefix(state.TokenText.Substring(6).Trim());
        }

        private static void HandleEndWith(HandlebarsParserState state)
        {
            state.PopExpressionPrefix();
        }

        private static void HandlePartial(HandlebarsParserState state)
        {
            var partialName = state.TokenText.Substring(1).Trim();
            var self = Expression.Self(state.Scopes.GetTypeOfModelInScope());
            state.Scopes.AddToCurrentScope(SyntaxTree.Include(partialName, self));
        }

        private static void HandleMaster(HandlebarsParserState state)
        {
            state.Scopes.AssertSyntaxTreeIsEmpty();
            var masterName = state.TokenText.Substring(1).Trim();
            state.ExtendNode = SyntaxTree.Extend(masterName, new Dictionary<string, SyntaxTreeNode>
            {
                {"body", state.Scopes.GetCurrentBlock()}
            });
        }

        private static void HandleBody(HandlebarsParserState state)
        {
            state.Scopes.AddToCurrentScope(SyntaxTree.Override("body"));
        }

        private static void HandleExpression(HandlebarsParserState state)
        {
            var expression = state.ParseExpression(state.TokenText);
            state.Scopes.AddToCurrentScope(SyntaxTree.WriteExpression(expression, state.HtmlEscape));
        }
    }
}