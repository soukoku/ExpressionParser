using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser.Parsing
{

    /// <summary>
    /// A low-level token split from the initial text input.
    /// </summary>
    public class RawToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawToken"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="position">The position.</param>
        internal RawToken(RawTokenType type, int position)
        {
            TokenType = type;
            Position = position;
            ValueBuilder = new StringBuilder();
        }

        /// <summary>
        /// Gets the token type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public RawTokenType TokenType { get; private set; }

        /// <summary>
        /// Gets the starting position of this token in the original input.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position { get; private set; }

        // TODO: test pef on using builder or using string directly
        internal StringBuilder ValueBuilder { get; private set; }

        internal void Append(RawToken token)
        {
            if (token != null)
            {
                ValueBuilder.Append(token.ValueBuilder);
            }
        }

        /// <summary>
        /// Gets the token value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get { return ValueBuilder.ToString(); } }

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
    }

    /// <summary>
    /// Indicates the low-level token type.
    /// </summary>
    public enum RawTokenType
    {
        /// <summary>
        /// Invalid token type.
        /// </summary>
        None,
        /// <summary>
        /// Token is white space.
        /// </summary>
        WhiteSpace,
        /// <summary>
        /// Token is a symbol.
        /// </summary>
        Symbol,
        /// <summary>
        /// Token is not symbol or white space.
        /// </summary>
        Literal,
    }
}
