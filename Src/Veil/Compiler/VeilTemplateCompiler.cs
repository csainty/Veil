using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private readonly ParameterExpression writer = Expression.Parameter(typeof(TextWriter), "writer");
        private readonly ParameterExpression model = Expression.Parameter(typeof(T), "model");
        private LinkedList<Expression> modelStack = new LinkedList<Expression>();
        private readonly Func<string, Type, Veil.Parser.SyntaxTreeNode> includeParser;
        private readonly IDictionary<string, Veil.Parser.SyntaxTreeNode> overrideSections = new Dictionary<string, Veil.Parser.SyntaxTreeNode>();

        public VeilTemplateCompiler(Func<string, Type, Veil.Parser.SyntaxTreeNode> includeParser)
        {
            this.includeParser = includeParser;
        }

        public Action<TextWriter, T> Compile(Parser.SyntaxTreeNode templateSyntaxTree)
        {
            while (templateSyntaxTree is Veil.Parser.Nodes.ExtendTemplateNode)
            {
                templateSyntaxTree = Extend((Veil.Parser.Nodes.ExtendTemplateNode)templateSyntaxTree);
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

        private Veil.Parser.SyntaxTreeNode Extend(Veil.Parser.Nodes.ExtendTemplateNode extendNode)
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