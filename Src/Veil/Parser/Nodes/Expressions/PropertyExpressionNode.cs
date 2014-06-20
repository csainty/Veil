using System;
using System.Reflection;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that evaluates a property on the model
    /// </summary>
    public class PropertyExpressionNode : ExpressionNode
    {
        public PropertyInfo PropertyInfo { get; set; }

        public override Type ResultType
        {
            get { return this.PropertyInfo.PropertyType; }
        }
    }
}