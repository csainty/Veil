using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sigil;

namespace Veil.Compiler
{
    internal static class EmitterExtensions
    {
        private static readonly IDictionary<Type, MethodInfo> writers = new Dictionary<Type, MethodInfo>
        {
            { typeof(string), typeof(TextWriter).GetMethod("Write", new[] { typeof(string) }) },
            { typeof(int), typeof(TextWriter).GetMethod("Write", new[] { typeof(int) }) },
            { typeof(double), typeof(TextWriter).GetMethod("Write", new[] { typeof(double) }) },
            { typeof(float), typeof(TextWriter).GetMethod("Write", new[] { typeof(float) }) },
            { typeof(long), typeof(TextWriter).GetMethod("Write", new[] { typeof(long) }) },
            { typeof(uint), typeof(TextWriter).GetMethod("Write", new[] { typeof(uint) }) },
            { typeof(ulong), typeof(TextWriter).GetMethod("Write", new[] { typeof(ulong) }) },
        };

        public static void LoadWriterToStack<T>(this Emit<Action<TextWriter, T>> emitter)
        {
            emitter.LoadArgument(0);
        }

        public static void CallMethod<T>(this Emit<Action<TextWriter, T>> emitter, MethodInfo info)
        {
            if (info.IsVirtual)
            {
                emitter.CallVirtual(info);
            }
            else
            {
                emitter.Call(info);
            }
        }

        public static void CallWriteFor<T>(this Emit<Action<TextWriter, T>> emitter, Type writerType)
        {
            emitter.CallMethod(writers[writerType]);
        }

        public static void OutputLiteral<T>(this Emit<Action<TextWriter, T>> emitter, string content)
        {
            emitter.LoadWriterToStack();
            emitter.LoadConstant(content);
            emitter.CallWriteFor(typeof(string));
        }

        public static void OutputLiteral<T>(this Emit<Action<TextWriter, T>> emitter, int content)
        {
            emitter.LoadWriterToStack();
            emitter.LoadConstant(content);
            emitter.CallWriteFor(typeof(int));
        }

        public static void OutputLiteral<T>(this Emit<Action<TextWriter, T>> emitter, double content)
        {
            emitter.LoadWriterToStack();
            emitter.LoadConstant(content);
            emitter.CallWriteFor(typeof(double));
        }

        public static void OutputLiteral<T>(this Emit<Action<TextWriter, T>> emitter, float content)
        {
            emitter.LoadWriterToStack();
            emitter.LoadConstant(content);
            emitter.CallWriteFor(typeof(float));
        }

        public static void OutputLiteral<T>(this Emit<Action<TextWriter, T>> emitter, long content)
        {
            emitter.LoadWriterToStack();
            emitter.LoadConstant(content);
            emitter.CallWriteFor(typeof(long));
        }

        public static void OutputLiteral<T>(this Emit<Action<TextWriter, T>> emitter, uint content)
        {
            emitter.LoadWriterToStack();
            emitter.LoadConstant(content);
            emitter.CallWriteFor(typeof(uint));
        }

        public static void OutputLiteral<T>(this Emit<Action<TextWriter, T>> emitter, ulong content)
        {
            emitter.LoadWriterToStack();
            emitter.LoadConstant(content);
            emitter.CallWriteFor(typeof(ulong));
        }

        public static void LoadExpressionFromCurrentModelOnStack<T>(this Emit<Action<TextWriter, T>> emitter, SyntaxTreeNode.ExpressionNode expression)
        {
            if (expression is SyntaxTreeNode.ExpressionNode.PropertyExpressionNode)
            {
                emitter.CallMethod(((SyntaxTreeNode.ExpressionNode.PropertyExpressionNode)expression).Property.GetGetMethod());
            }
            else if (expression is SyntaxTreeNode.ExpressionNode.FieldExpressionNode)
            {
                emitter.LoadField(((SyntaxTreeNode.ExpressionNode.FieldExpressionNode)expression).Field);
            }
            else if (expression is SyntaxTreeNode.ExpressionNode.SubModelExpressionNode)
            {
                emitter.LoadExpressionFromCurrentModelOnStack(((SyntaxTreeNode.ExpressionNode.SubModelExpressionNode)expression).ModelExpression);
                emitter.LoadExpressionFromCurrentModelOnStack(((SyntaxTreeNode.ExpressionNode.SubModelExpressionNode)expression).SubModelExpression);
            }
            else if (expression is SyntaxTreeNode.ExpressionNode.FunctionCallExpressionNode)
            {
                emitter.CallMethod(((SyntaxTreeNode.ExpressionNode.FunctionCallExpressionNode)expression).Function);
            }
            else if (expression is SyntaxTreeNode.ExpressionNode.SelfExpressionNode)
            {
            }
            else
            {
                throw new VeilCompilerException("Unknown expression type '{0}'".FormatInvariant(expression.GetType().Name));
            }
        }
    }
}