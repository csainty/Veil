using System;
using System.Linq.Expressions;

namespace Veil.Compiler
{
    public struct TypedExpression
    {
        private Expression expression;
        private Type type;

        public TypedExpression(Expression expression, Type type)
        {
            this.expression = expression;
            this.type = type;
        }

        public Expression Expression { get { return this.expression; } }

        public Type Type { get { return this.type; } }
    }
}