namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// Used to indicate how to handle resolve field value.
    /// </summary>
    public enum ValueTypeHint
    {
        /// <summary>
        /// Value is converted to suitable type for comparison purposes.
        /// </summary>
        Auto,
        /// <summary>
        /// Value is forced to be text for comparison purposes.
        /// </summary>
        Text,
    }
}
