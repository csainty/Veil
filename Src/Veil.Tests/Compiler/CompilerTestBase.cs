using System.IO;

namespace Veil.Compiler
{
    internal class CompilerTestBase
    {
        private readonly ITemplateCompiler compiler = new VeilTemplateCompiler();

        protected string ExecuteTemplate<T>(TemplateRootNode syntaxTree, T model)
        {
            var template = this.compiler.Compile<T>(syntaxTree);
            using (var writer = new StringWriter())
            {
                template(writer, model);
                return writer.ToString();
            }
        }

        protected TemplateRootNode CreateTemplate(params ISyntaxTreeNode[] nodes)
        {
            var template = new TemplateRootNode();
            template.AddRange(nodes);
            return template;
        }

        protected BlockNode CreateBlock(params ISyntaxTreeNode[] nodes)
        {
            var block = new BlockNode();
            block.AddRange(nodes);
            return block;
        }

        protected WriteLiteralNode CreateStringLiteral(string value)
        {
            return new WriteLiteralNode { LiteralType = typeof(string), LiteralContent = value };
        }
    }
}