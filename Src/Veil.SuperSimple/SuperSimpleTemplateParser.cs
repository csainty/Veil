using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Parser;

namespace Veil.SuperSimple
{
    internal static class SuperSimpleTemplateParser
    {
        public static SyntaxTreeNode Parse(IEnumerable<SuperSimpleToken> tokens, Type modelType)
        {
            var scopeStack = new LinkedList<SuperSimpleParserScope>();
            scopeStack.AddFirst(new SuperSimpleParserScope { Block = SyntaxTree.Block(), ModelType = modelType });

            foreach (var token in tokens)
            {
                if (!token.IsSyntaxToken)
                {
                    scopeStack.First().Block.Add(SyntaxTree.WriteString(token.Content));
                    continue;
                }

                var currentToken = token.Content.Trim(new[] { '@', ';' });
                if (currentToken == "Each")
                {
                    var each = SyntaxTree.Iterate(
                        Expression.Self(scopeStack.First.Value.ModelType),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new SuperSimpleParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (currentToken.StartsWith("Each"))
                {
                    currentToken = currentToken.Substring(5);
                    var each = SyntaxTree.Iterate(
                        SuperSimpleExpressionParser.Parse(scopeStack, currentToken),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(each);
                    scopeStack.AddFirst(new SuperSimpleParserScope { Block = each.Body, ModelType = each.ItemType });
                }
                else if (currentToken == "EndEach")
                {
                    scopeStack.RemoveFirst();
                }
                else if (currentToken.StartsWith("If.") || currentToken.StartsWith("IfNotNull."))
                {
                    currentToken = currentToken.Substring(currentToken.IndexOf('.') + 1);
                    var condition = SyntaxTree.Conditional(
                        SuperSimpleExpressionParser.Parse(scopeStack, currentToken),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(condition);
                    scopeStack.AddFirst(new SuperSimpleParserScope { Block = condition.TrueBlock, ModelType = scopeStack.First.Value.ModelType });
                }
                else if (currentToken.StartsWith("IfNot.") || currentToken.StartsWith("IfNull."))
                {
                    currentToken = currentToken.Substring(currentToken.IndexOf('.') + 1);
                    var condition = SyntaxTree.Conditional(
                        SuperSimpleExpressionParser.Parse(scopeStack, currentToken),
                        SyntaxTree.Block(),
                        SyntaxTree.Block()
                    );
                    scopeStack.First.Value.Block.Add(condition);
                    scopeStack.AddFirst(new SuperSimpleParserScope { Block = condition.FalseBlock, ModelType = scopeStack.First.Value.ModelType });
                }
                else if (currentToken == "EndIf")
                {
                    scopeStack.RemoveFirst();
                }
                else if (currentToken.StartsWith("Partial"))
                {
                    var details = SuperSimpleNameModelParser.Parse(currentToken);
                    ExpressionNode modelExpression = Expression.Self(scopeStack.First.Value.ModelType);

                    if (!String.IsNullOrEmpty(details.Model))
                    {
                        modelExpression = SuperSimpleExpressionParser.Parse(scopeStack, details.Model);
                    }
                    scopeStack.First.Value.Block.Add(SyntaxTree.Include(details.Name, modelExpression));
                }
                else if (currentToken.StartsWith("Section"))
                {
                    scopeStack.First.Value.Block.Add(SyntaxTree.Override(SuperSimpleNameModelParser.Parse(currentToken).Name));
                }
                else if (currentToken == "Flush")
                {
                    scopeStack.First.Value.Block.Add(SyntaxTree.Flush());
                }
                else if (currentToken.StartsWith("!"))
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack, currentToken.Substring(1));
                    scopeStack.First.Value.Block.Add(SyntaxTree.WriteExpression(expression, true));
                }
                else
                {
                    var expression = SuperSimpleExpressionParser.Parse(scopeStack, currentToken);
                    scopeStack.First.Value.Block.Add(SyntaxTree.WriteExpression(expression));
                }
            }

            return scopeStack.First.Value.Block;
        }
    }
}