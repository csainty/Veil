using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Veil.Parser;

namespace Veil.Compiler
{
    internal class CompilerTestBase
    {
        private readonly Dictionary<string, SyntaxTreeNode> templates = new Dictionary<string, SyntaxTreeNode>();

        [SetUp]
        public void SetUp()
        {
            templates.Clear();
        }

        protected string ExecuteTemplate<T>(SyntaxTreeNode syntaxTree, T model)
        {
            var template = new VeilTemplateCompiler<T>(GetTemplateByName).Compile(syntaxTree);
            using (var writer = new StringWriter())
            {
                template(writer, model);
                return writer.ToString();
            }
        }

        protected void RegisterTemplate(string name, SyntaxTreeNode syntaxTree)
        {
            templates.Add(name, syntaxTree);
        }

        public SyntaxTreeNode GetTemplateByName(string name, Type modelType)
        {
            if (!templates.ContainsKey(name))
                return null;
            return templates[name];
        }
    }
}