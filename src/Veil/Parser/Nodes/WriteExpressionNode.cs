namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that evaluates an expression and writes its result to the <see cref="System.IO.TextWriter"/>
    /// </summary>
    public class WriteExpressionNode : SyntaxTreeNode
    {
        /// <summary>
        /// An expression which is evaluated and the result written to the output of the template
        /// </summary>
        public ExpressionNode Expression { get; set; }

        /// <summary>
        /// Indicates whether to execute an HtmlEncode over the result of the expression
        /// </summary>
        public bool HtmlEncode { get; set; }
    }
}