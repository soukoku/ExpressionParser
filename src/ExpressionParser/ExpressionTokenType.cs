using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// Indicates the expression token type.
    /// </summary>
    public enum ExpressionTokenType
    {
        /// <summary>
        /// Invalid token type.
        /// </summary>
        None,
        /// <summary>
        /// The token is an operator.
        /// </summary>
        Operator,
        /// <summary>
        /// The token is an open parenthesis.
        /// </summary>
        OpenParenthesis,
        /// <summary>
        /// The token is a close parenthesis.
        /// </summary>
        CloseParenthesis,
        /// <summary>
        /// The token is a function.
        /// </summary>
        Function,
        /// <summary>
        /// The token is a comma.
        /// </summary>
        Comma,
        /// <summary>
        /// The token is a field reference.
        /// </summary>
        Field,
        /// <summary>
        /// The token is from single quoted value.
        /// </summary>
        SingleQuoted,
        /// <summary>
        /// The token is from double quoted value.
        /// </summary>
        DoubleQuoted,
        /// <summary>
        /// The token is a yet-to-be-parsed value.
        /// </summary>
        Value,
    }
}
