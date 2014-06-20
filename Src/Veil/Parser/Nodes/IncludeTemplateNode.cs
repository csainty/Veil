namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node which executes another template in the context of the supplied model
    /// </summary>
    public class IncludeTemplateNode : SyntaxTreeNode
    {
        public ExpressionNode ModelExpression { get; set; }

        public string TemplateName { get; set; }
    }
}