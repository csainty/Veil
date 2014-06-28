using System;
using System.Reflection;

namespace Veil.Parser.Nodes
{
    /// <summary>
    /// Expression that executes a function on the model
    /// </summary>
    public class FunctionCallExpressionNode : ExpressionNode
    {
        /// <summary>
        /// The function to execute
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary>
        /// The type of result from this expression evaluation
        /// </summary>
        public override Type ResultType
        {
            get { return this.MethodInfo.ReturnType; }
        }
    }
}