namespace Veil.Parser
{
    /// <summary>
    /// Defines the available scopes that an expression can be referencing
    /// </summary>
    public enum ExpressionScope
    {
        /// <summary>
        /// Scoped to the current model that is on the stack
        /// </summary>
        CurrentModelOnStack,

        /// <summary>
        /// Scoped to the root model of this template
        /// </summary>
        RootModel,

        /// <summary>
        /// Scoped to the model of the parent scope block. E.g. The outer scope when inside an iteration block
        /// </summary>
        ModelOfParentScope
    }
}