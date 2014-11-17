using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal static class HandlebarsExpressionParser
    {
        public static ExpressionNode Parse(HandlebarsBlockStack blockStack, string expression)
        {
            expression = expression.Trim();

            if (expression == "this")
            {
                return SyntaxTreeExpression.Self(blockStack.GetCurrentModelType(), ExpressionScope.CurrentModelOnStack);
            }
            if (expression.StartsWith("../"))
            {
                return ParseAgainstModel(blockStack.GetParentModelType(), expression.Substring(3), ExpressionScope.ModelOfParentScope);
            }

            return ParseAgainstModel(blockStack.GetCurrentModelType(), expression, ExpressionScope.CurrentModelOnStack);
        }

        private static ExpressionNode ParseAgainstModel(Type modelType, string expression, ExpressionScope expressionScope)
        {
            var dotIndex = expression.IndexOf('.');
            if (dotIndex >= 0)
            {
                var subModel = HandlebarsExpressionParser.ParseAgainstModel(modelType, expression.Substring(0, dotIndex), expressionScope);
                return SyntaxTreeExpression.SubModel(
                    subModel,
                    HandlebarsExpressionParser.ParseAgainstModel(subModel.ResultType, expression.Substring(dotIndex + 1), ExpressionScope.CurrentModelOnStack)
                );
            }

            if (expression.EndsWith("()"))
            {
                var func = FindMember(modelType, expression.Substring(0, expression.Length - 2), MemberTypes.Method);
                if (func != null) return SyntaxTreeExpression.Function(modelType, func.Name, expressionScope);
            }

            var prop = FindMember(modelType, expression, MemberTypes.Property | MemberTypes.Field);
            if (prop != null)
            {
                switch (prop.MemberType)
                {
                    case MemberTypes.Property: return SyntaxTreeExpression.Property(modelType, prop.Name, expressionScope);
                    case MemberTypes.Field: return SyntaxTreeExpression.Field(modelType, prop.Name, expressionScope);
                }
            }

            if (IsLateBoundAcceptingType(modelType)) return SyntaxTreeExpression.LateBound(expression, false, expressionScope);

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}' againt model '{1}'", expression, modelType.Name));
        }

        private static MemberInfo FindMember(Type t, string name, MemberTypes types)
        {
            return t
                .FindMembers(types, BindingFlags.Instance | BindingFlags.Public, Type.FilterNameIgnoreCase, name)
                .OrderByDescending(x => x.Name == name)
                .ThenByDescending(x => x.MemberType == MemberTypes.Property)
                .FirstOrDefault();
        }

        private static bool IsLateBoundAcceptingType(Type type)
        {
            return type == typeof(object) 
                || type.IsDictionary()
                || type.GetInterfaces().Any(IsDictionary)
                || type.GetProperties().Any(p => p.GetIndexParameters().Any());
        }

        private static bool IsDictionary(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }
    }
}