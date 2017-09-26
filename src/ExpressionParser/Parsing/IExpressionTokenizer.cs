namespace Soukoku.ExpressionParser.Parsing
{
    /// <summary>
    /// Interface for something that can tokenize an expression.
    /// </summary>
    public interface IExpressionTokenizer
    {
        /// <summary>
        /// Splits the specified input expression into a list of <see cref="ExpressionToken" /> values.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        ExpressionToken[] Tokenize(string input);
    }
}