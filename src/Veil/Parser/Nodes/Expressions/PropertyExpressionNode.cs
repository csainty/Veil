using System;
using System.Reflection;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates a property on the model
    /// </summary>
    public class PropertyExpressionNode : ExpressionNode
    {
        /// <summary>
        /// The property to evaluate
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// The type of result from this expression evaluation
        /// </summary>
        public override Type ResultType
        {
            get { return this.PropertyInfo.PropertyType; }
        }
    }
}