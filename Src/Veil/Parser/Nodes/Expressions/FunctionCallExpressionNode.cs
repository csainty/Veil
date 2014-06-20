using System;
using System.Reflection;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that executes a function on the model
    /// </summary>
    public class FunctionCallExpressionNode : ExpressionNode
    {
        public MethodInfo MethodInfo { get; set; }

        public override Type ResultType
        {
            get { return this.MethodInfo.ReturnType; }
        }
    }
}