using System.IO;

namespace Veil.Compiler
{
    internal class CompilerTestBase
    {
        protected string ExecuteTemplate<T>(SyntaxTreeNode syntaxTree, T model)
        {
            var template = new VeilTemplateCompiler<T>().Compile(syntaxTree);
            using (var writer = new StringWriter())
            {
                template(writer, model);
                return writer.ToString();
            }
        }
    }
}