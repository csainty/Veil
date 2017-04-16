namespace Veil.Parser.Nodes
{
    /// <summary>
    /// A node which executes another template in the context of the supplied model
    /// </summary>
    public class IncludeTemplateNode : SyntaxTreeNode
    {
        /// <summary>
        /// Expression which evaluates to the model to execute the template with
        /// </summary>
        public ExpressionNode ModelExpression { get; set; }

        /// <summary>
        /// The name of the template to load via <see cref="IVeilContext.GetTemplateByName"/>
        /// </summary>
        public string TemplateName { get; set; }
    }
}