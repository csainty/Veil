namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node that evaluates an expression and executes one of the blocks based on the truthy-ness of the result
    /// </summary>
    public class ConditionalNode : SyntaxTreeNode
    {
        private ExpressionNode expression;

        /// <summary>
        /// The expression which will be evaluated to determine block to execute
        /// </summary>
        public ExpressionNode Expression { get { return this.expression; } set { this.expression = value; this.Validate(); } }

        /// <summary>
        /// The block to execute when the expression evaluates truthy
        /// </summary>
        public BlockNode TrueBlock { get; set; }

        /// <summary>
        /// The block to execute when the expression evaluates falsey
        /// </summary>
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