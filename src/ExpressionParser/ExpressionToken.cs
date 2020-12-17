using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Soukoku.ExpressionParser.Parsing;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// A token split from the initial text input.
    /// </summary>
    public class ExpressionToken
    {
        /// <summary>
        /// Canonical true value. Actual value is numerical "1".
        /// </summary>
        public static readonly ExpressionToken True = new ExpressionToken("1");
        /// <summary>
        /// Canonical false value. Actual value is numerical "0".
        /// </summary>
        public static readonly ExpressionToken False = new ExpressionToken("0");

        internal static readonly NumberStyles NumberParseStyle = NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol | NumberStyles.Number;

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

            _value = _rawToken?.ToString();
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
        /// Gets the raw token value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get { return _value ?? _rawToken?.ToString(); } }

        /// <summary>
        /// Gets the resolved field value and type hint if token is a field.
        /// </summary>
        public (object Value, ValueTypeHint TypeHint) FieldValue { get; internal set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            switch (TokenType)
            {
                case ExpressionTokenType.Field:
                    return FieldValue.Value?.ToString() ?? "";
                default:
                    return Value ?? "";
            }
        }


        #region conversion routines

        ///// <summary>
        ///// Check if the value is considered numeric.
        ///// </summary>
        ///// <returns></returns>
        //public bool IsNumeric()
        //{
        //    if (TokenType == ExpressionTokenType.Field && FieldValue.TypeHint == ValueTypeHint.Text) return false;

        //    return decimal.TryParse(Value, NumberParseStyle, CultureInfo.InvariantCulture, out decimal dummy);
        //}

        ///// <summary>
        ///// Check if the value is considered true.
        ///// </summary>
        ///// <returns></returns>
        //public bool IsTrue(string value)
        //{
        //    if (TokenType == ExpressionTokenType.Field && FieldValue.TypeHint == ValueTypeHint.Text) return false;

        //    return string.Equals("true", Value, StringComparison.OrdinalIgnoreCase) || value == "1";
        //}

        ///// <summary>
        ///// Check if the value is considered false.
        ///// </summary>
        ///// <returns></returns>
        //public bool IsFalse(string value)
        //{
        //    if (TokenType == ExpressionTokenType.Field && FieldValue.TypeHint == ValueTypeHint.Text) return false;

        //    return string.Equals("false", Value, StringComparison.OrdinalIgnoreCase) || value == "0";
        //}

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
                    return double.Parse(Value, NumberParseStyle, CultureInfo.InvariantCulture);
                case ExpressionTokenType.Field:
                    return double.Parse(FieldValue.Value?.ToString(), NumberParseStyle, CultureInfo.InvariantCulture);
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
                    return decimal.Parse(Value, NumberParseStyle, CultureInfo.InvariantCulture);
                case ExpressionTokenType.Field:
                    return decimal.Parse(FieldValue.Value?.ToString(), NumberParseStyle, CultureInfo.InvariantCulture);
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Cannot convert {0}({1}) to a numeric value.", TokenType, Value));
            }
        }

        #endregion
    }


}
