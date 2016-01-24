using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// Context for storing and returning values during an expression evaluation.
    /// </summary>
    public class EvaluationContext
    {
        internal decimal ToDecimal(ExpressionToken token)
        {
            switch (token.TokenType)
            {
                case ExpressionTokenType.Value:
                case ExpressionTokenType.SingleQuoted:
                case ExpressionTokenType.DoubleQuoted:
                    return decimal.Parse(token.Value);
                case ExpressionTokenType.Field:
                    return 0;
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Cannot convert {0}({1}) to a numeric value.", token.TokenType, token.Value));
            }
        }
    }
}
