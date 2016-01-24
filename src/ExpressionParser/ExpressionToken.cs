using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soukoku.ExpressionParser.Utilities;
using System.Globalization;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// A token split from the initial text input.
    /// </summary>
    public class ExpressionToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionToken"/> class.
        /// </summary>
        public ExpressionToken() { }

        /// <summary>
        /// Initializes a new frozen instance of the <see cref="ExpressionToken"/> class
        /// with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExpressionToken(string value)
        {
            _type = ExpressionTokenType.Value;
            _value = value;
        }


        RawToken _rawToken; // the raw token that makes this token

        /// <summary>
        /// Gets the raw token that made this list.
        /// </summary>
        /// <returns></returns>
        public RawToken RawToken { get { return _rawToken; } }

        const string FrozenErrorMsg = "Cannot modify frozen token.";

        /// <summary>
        /// Appends the specified token to this expression.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Append(RawToken token)
        {
            if (IsFrozen) { throw new InvalidOperationException(FrozenErrorMsg); }

            if (_rawToken == null) { _rawToken = token; }
            else { _rawToken.Append(token); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is frozen from append.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is frozen; otherwise, <c>false</c>.
        /// </value>
        public bool IsFrozen { get { return _value != null; } }

        /// <summary>
        /// Freezes this instance from being appended.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Freeze()
        {
            if (IsFrozen) { throw new InvalidOperationException(FrozenErrorMsg); }

            _value = _rawToken.ToString();
        }

        private ExpressionTokenType _type;
        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        public ExpressionTokenType TokenType
        {
            get { return _type; }
            set { if (_value == null) { _type = value; } }
        }

        /// <summary>
        /// Gets or sets the type of the operator. This is only used if the <see cref="TokenType"/>
        /// is <see cref="ExpressionTokenType.Operator"/>.
        /// </summary>
        /// <value>
        /// The type of the operator.
        /// </value>
        public OperatorType OperatorType { get; set; }

        string _value;

        /// <summary>
        /// Gets the token value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get { return _value ?? _rawToken.ToString(); } }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }


        #region conversion routines

        /// <summary>
        /// Gets value as string with resolution.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public string ToString(EvaluationContext context)
        {
            switch (TokenType)
            {
                case ExpressionTokenType.Field:
                    if (context == null) { throw new ArgumentNullException("context"); }
                    return context.GetFieldValue(Value).ToString();
                default:
                    return Value;
            }
        }

        /// <summary>
        /// Converts to the double value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="FormatException"></exception>
        public double ToDouble(EvaluationContext context)
        {
            switch (TokenType)
            {
                case ExpressionTokenType.Value:
                case ExpressionTokenType.SingleQuoted:
                case ExpressionTokenType.DoubleQuoted:
                    return double.Parse(Value, CultureInfo.CurrentCulture);
                case ExpressionTokenType.Field:
                    if (context == null) { throw new ArgumentNullException("context"); }
                    return double.Parse(context.GetFieldValue(Value).ToString(), CultureInfo.CurrentCulture);
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Cannot convert {0}({1}) to a numeric value.", TokenType, Value));
            }
        }

        /// <summary>
        /// Converts to the decimal value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="FormatException"></exception>
        public decimal ToDecimal(EvaluationContext context)
        {
            switch (TokenType)
            {
                case ExpressionTokenType.Value:
                case ExpressionTokenType.SingleQuoted:
                case ExpressionTokenType.DoubleQuoted:
                    return decimal.Parse(Value, CultureInfo.CurrentCulture);
                case ExpressionTokenType.Field:
                    if (context == null) { throw new ArgumentNullException("context"); }
                    return decimal.Parse(context.GetFieldValue(Value).ToString(), CultureInfo.CurrentCulture);
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Cannot convert {0}({1}) to a numeric value.", TokenType, Value));
            }
        }

        #endregion
    }


}
