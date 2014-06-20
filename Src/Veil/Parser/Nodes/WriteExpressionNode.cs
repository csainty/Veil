namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that evaluates an expression and writes its result to the <see cref="TextWriter"/>
    /// </summary>
    public class WriteExpressionNode : SyntaxTreeNode
    {
        public ExpressionNode Expression { get; set; }

        public bool HtmlEncode { get; set; }
    }
}