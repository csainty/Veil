using System;
using System.Reflection;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Experssion that evaluates a field on the model
    /// </summary>
    public class FieldExpressionNode : ExpressionNode
    {
        /// <summary>
        /// The field to evaluate
        /// </summary>
        public FieldInfo FieldInfo { get; set; }

        /// <summary>
        /// The type of result from this expression evaluation
        /// </summary>
        public override Type ResultType
        {
            get { return this.FieldInfo.FieldType; }
        }
    }
}