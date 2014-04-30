using System;
using System.IO;

namespace Veil.Compiler
{
    internal class CompilerTestBase
    {
        protected string ExecuteTemplate<T>(SyntaxTreeNode syntaxTree, T model)
        {
            var template = new VeilTemplateCompiler<T>(GetTemplateByName).Compile(syntaxTree);
            using (var writer = new StringWriter())
            {
                template(writer, model);
                return writer.ToString();
            }
        }

        public SyntaxTreeNode GetTemplateByName(string name, Type modelType)
        {
            throw new System.NotImplementedException();
        }
    }
}