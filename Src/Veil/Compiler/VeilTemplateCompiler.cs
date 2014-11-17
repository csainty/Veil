using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private readonly ParameterExpression writer = Expression.Parameter(typeof(TextWriter), "writer");
        private readonly ParameterExpression model = Expression.Parameter(typeof(T), "model");
        private LinkedList<Expression> modelStack = new LinkedList<Expression>();
        private readonly Func<string, Type, SyntaxTreeNode> includeParser;
        private readonly IDictionary<string, SyntaxTreeNode> overrideSections = new Dictionary<string, Veil.Parser.SyntaxTreeNode>();

        public VeilTemplateCompiler(Func<string, Type, SyntaxTreeNode> includeParser)
        {
            this.includeParser = includeParser;
        }

        public Action<TextWriter, T> Compile(SyntaxTreeNode templateSyntaxTree)
        {
            while (templateSyntaxTree is ExtendTemplateNode)
            {
                templateSyntaxTree = Extend((ExtendTemplateNode)templateSyntaxTree);
            }

            this.PushScope(model);
            return Expression.Lambda<Action<TextWriter, T>>(Node(templateSyntaxTree), writer, model).Compile();
        }

        private void PushScope(Expression scope)
        {
            this.modelStack.AddFirst(scope);
        }

        private void PopScope()
        {
            this.modelStack.RemoveFirst();
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
    }
}