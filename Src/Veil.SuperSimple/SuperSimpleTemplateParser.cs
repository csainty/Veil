using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleTemplateParser
    {
        private static readonly Dictionary<Func<SuperSimpleToken, bool>, Action<SuperSimpleTemplateParserState>> handlers = new Dictionary<Func<SuperSimpleToken, bool>, Action<SuperSimpleTemplateParserState>>
        {
            { t => !t.IsSyntaxToken, HandleStringLiteral },
            { t => t.Content == "Each", HandleEachOverSelf },
            { t => t.Content.StartsWith("Each"), HandleEachOverExpression },
            { t => t.Content == "EndEach", HandleEndEach },
            { t => t.Content.StartsWith("If.") || t.Content.StartsWith("IfNotNull."), HandlePositiveConditional },
            { t => t.Content.StartsWith("IfNot.") || t.Content.StartsWith("IfNull."), HandleNegativeConditional },
            { t => t.Content == "EndIf", HandleEndConditional },
            { t => t.Content.StartsWith("Partial"), HandlePartial },
            { t => t.Content.StartsWith("Section"), HandleSection },
            { t => t.Content == "Flush", HandleFlush },
            { t => true, HandleWriteLiteral }
        };

        public static SyntaxTreeNode Parse(IEnumerable<SuperSimpleToken> tokens, Type modelType)
        {
            var state = new SuperSimpleTemplateParserState();
            state.PushNewScope(modelType);

            foreach (var token in tokens)
            {
                state.CurrentToken = token;

                foreach (var handler in handlers)
                {
                    if (handler.Key(token))
                    {
                        handler.Value(state);
                        break;
                    }
                }
            }

            state.AssertScopeStackIsBackToASingleScope();
            return state.CurrentBlock;
        }

        private static void HandleStringLiteral(SuperSimpleTemplateParserState state)
        {
            state.AddNodeToCurrentBlock(SyntaxTree.WriteString(state.CurrentToken.Content));
        }

        private static void HandleEachOverSelf(SuperSimpleTemplateParserState state)
        {
            var each = SyntaxTree.Iterate(
                SyntaxTreeExpression.Self(state.CurrentTypeInScope()),
                SyntaxTree.Block()
            );
            state.AddNodeToCurrentBlock(each);
            state.PushNewScope(each.Body, each.ItemType);
        }

        private static void HandleEachOverExpression(SuperSimpleTemplateParserState state)
        {
            var each = SyntaxTree.Iterate(
                state.ParseCurrentTokenExpression(),
                SyntaxTree.Block()
            );
            state.AddNodeToCurrentBlock(each);
            state.PushNewScope(each.Body, each.ItemType);
        }

        private static void HandleEndEach(SuperSimpleTemplateParserState state)
        {
            state.AssertInsideIterationBlock();
            state.PopCurrentScope();
        }

        private static void HandlePositiveConditional(SuperSimpleTemplateParserState state)
        {
            var condition = SyntaxTree.Conditional(
                state.ParseCurrentTokenExpression(),
                SyntaxTree.Block()
            );
            state.AddNodeToCurrentBlock(condition);
            state.PushNewScope(condition.TrueBlock);
        }

        private static void HandleNegativeConditional(SuperSimpleTemplateParserState state)
        {
            var condition = SyntaxTree.Conditional(
                state.ParseCurrentTokenExpression(),
                SyntaxTree.Block(),
                SyntaxTree.Block()
            );
            state.AddNodeToCurrentBlock(condition);
            state.PushNewScope(condition.FalseBlock);
        }

        private static void HandleEndConditional(SuperSimpleTemplateParserState state)
        {
            state.AssertInsideConditionalBlock();
            state.PopCurrentScope();
        }

        private static void HandlePartial(SuperSimpleTemplateParserState state)
        {
            var details = state.ParseCurrentTokenNameAndModelExpression();
            ExpressionNode expression = SyntaxTreeExpression.Self(state.CurrentTypeInScope());

            if (!String.IsNullOrEmpty(details.Model))
            {
                expression = state.ParseExpression(details.Model);
            }
            state.AddNodeToCurrentBlock(SyntaxTree.Include(details.Name, expression));
        }

        private static void HandleSection(SuperSimpleTemplateParserState state)
        {
            var details = state.ParseCurrentTokenNameAndModelExpression();
            state.AddNodeToCurrentBlock(SyntaxTree.Override(details.Name));
        }

        private static void HandleFlush(SuperSimpleTemplateParserState state)
        {
            state.AddNodeToCurrentBlock(SyntaxTree.Flush());
        }

        private static void HandleWriteLiteral(SuperSimpleTemplateParserState state)
        {
            var expression = state.CurrentToken.Content;
            var htmlEncode = false;
            if (expression.StartsWith("!"))
            {
                htmlEncode = true;
                expression = expression.Substring(1);
            }
            state.AddNodeToCurrentBlock(SyntaxTree.WriteExpression(state.ParseExpression(expression), htmlEncode));
        }
    }
}