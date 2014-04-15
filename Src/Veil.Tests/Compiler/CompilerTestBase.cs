using System.IO;

namespace Veil.Compiler
{
    internal class CompilerTestBase
    {
        private readonly ITemplateCompiler compiler = new VeilTemplateCompiler();

        protected string ExecuteTemplate<T>(SyntaxTreeNode syntaxTree, T model)
        {
            var template = this.compiler.Compile<T>(syntaxTree);
            using (var writer = new StringWriter())
            {
                template(writer, model);
                return writer.ToString();
            }
        }
    }
}