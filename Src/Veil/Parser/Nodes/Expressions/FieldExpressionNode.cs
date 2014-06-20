using System;
using System.Reflection;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Experssion that evaluates a field on the model
    /// </summary>
    public class FieldExpressionNode : ExpressionNode
    {
        public FieldInfo FieldInfo { get; set; }

        public override Type ResultType
        {
            get { return this.FieldInfo.FieldType; }
        }
    }
}