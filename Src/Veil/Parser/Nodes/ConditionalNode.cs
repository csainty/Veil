namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that evaluates an expression and executes one of the blocks based on the truthy-ness of the result
    /// </summary>
    public class ConditionalNode : SyntaxTreeNode
    {
        private ExpressionNode expression;

        public ExpressionNode Expression { get { return this.expression; } set { this.expression = value; this.Validate(); } }

        public BlockNode TrueBlock { get; set; }

        public BlockNode FalseBlock { get; set; }

        private void Validate()
        {
            if (expression.ResultType.IsValueType && expression.ResultType != typeof(bool))
            {
                throw new VeilParserException("Attempted to use a ValueType other than bool as the expression in a conditional.");
            }
        }
    }
}