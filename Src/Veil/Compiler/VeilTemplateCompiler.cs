using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private LinkedList<Action<Emit<Action<TextWriter, T>>>> scopeStack;
        private readonly Emit<Action<TextWriter, T>> emitter;
        private readonly Func<string, Type, SyntaxTreeNode> includeParser;
        private readonly IDictionary<string, SyntaxTreeNode> overrideSections;

        public VeilTemplateCompiler(Func<string, Type, SyntaxTreeNode> includeParser)
        {
            scopeStack = new LinkedList<Action<Emit<Action<TextWriter, T>>>>();
            emitter = Emit<Action<TextWriter, T>>.NewDynamicMethod();
            overrideSections = new Dictionary<string, SyntaxTreeNode>();
            this.includeParser = includeParser;
        }

        public Action<TextWriter, T> Compile(SyntaxTreeNode templateSyntaxTree)
        {
            while (templateSyntaxTree is ExtendTemplateNode)
            {
                templateSyntaxTree = Extend((ExtendTemplateNode)templateSyntaxTree);
            }

            AddModelScope(e => e.LoadArgument(1));
            EmitNode(templateSyntaxTree);

            emitter.Return();
            return emitter.CreateDelegate();
        }

        private SyntaxTreeNode Extend(ExtendTemplateNode extendNode)
        {
            foreach (var o in extendNode.Overrides)
            {
                if (overrideSections.ContainsKey(o.Key)) continue;

                overrideSections.Add(o.Key, o.Value);
            }
            return includeParser(extendNode.TemplateName, typeof(T));
        }

        private IDisposable CreateLocalScopeStack()
        {
            var oldScopeStack = scopeStack;
            scopeStack = new LinkedList<Action<Emit<Action<TextWriter, T>>>>();
            return new ActionDisposable(() =>
            {
                scopeStack = oldScopeStack;
            });
        }

        private void AddModelScope(Action<Emit<Action<TextWriter, T>>> scope)
        {
            scopeStack.AddFirst(scope);
        }

        private void RemoveModelScope()
        {
            scopeStack.RemoveFirst();
        }

        private void PushCurrentModelOnStack()
        {
            scopeStack.First.Value.Invoke(emitter);
        }

        private void LoadWriterToStack()
        {
            emitter.LoadArgument(0);
        }

        private void CallWriteFor(Type typeOfItemOnStack)
        {
            if (!writers.ContainsKey(typeOfItemOnStack)) throw new VeilCompilerException("Unable to call TextWriter.Write() for item of type '{0}'".FormatInvariant(typeOfItemOnStack.Name));
            emitter.CallMethod(writers[typeOfItemOnStack]);
        }

        private static readonly IDictionary<Type, MethodInfo> writers = new Dictionary<Type, MethodInfo>
        {
            { typeof(string), typeof(TextWriter).GetMethod("Write", new[] { typeof(string) }) },
            { typeof(int), typeof(TextWriter).GetMethod("Write", new[] { typeof(int) }) },
            { typeof(double), typeof(TextWriter).GetMethod("Write", new[] { typeof(double) }) },
            { typeof(float), typeof(TextWriter).GetMethod("Write", new[] { typeof(float) }) },
            { typeof(long), typeof(TextWriter).GetMethod("Write", new[] { typeof(long) }) },
            { typeof(uint), typeof(TextWriter).GetMethod("Write", new[] { typeof(uint) }) },
            { typeof(ulong), typeof(TextWriter).GetMethod("Write", new[] { typeof(ulong) }) },
            { typeof(object), typeof(TextWriter).GetMethod("Write", new[] { typeof(object) }) },
        };
    }
}