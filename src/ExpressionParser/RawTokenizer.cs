using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// The initial input string tokenizer.
    /// </summary>
    public class RawTokenizer
    {
        static readonly char[] __defaultSymbols = new[]
            {
                '+', '-', '*', '/', '=', '%', '^',
                ',', '<', '>', '&', '|', '!',
                '(', ')', '{', '}', '[', ']'
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="RawTokenizer" /> class.
        /// </summary>
        /// <param name="symbols">The char values to count as symbols. If null a default set will be used.</param>
        public RawTokenizer(char[] symbols = null)
        {
            Symbols = symbols ?? __defaultSymbols;
        }

        /// <summary>
        /// Gets the char values that count as symbols.
        /// </summary>
        /// <value>
        /// The symbols.
        /// </value>
        public char[] Symbols { get; private set; }

        /// <summary>
        /// Splits the specified input into a list of raw token values using whitespace and symbols.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public IList<RawToken> Tokenize(string input)
        {
            var tokens = new List<RawToken>();

            if (input != null)
            {
                RawToken lastToken = null;
                for (int i = 0; i < input.Length; i++)
                {
                    var ch = input[i];
                    if (char.IsWhiteSpace(ch))
                    {
                        lastToken = NewTokenIfNecessary(tokens, lastToken, RawTokenType.Whitespace);
                    }
                    else if (Symbols.Contains(ch))
                    {
                        lastToken = NewTokenIfNecessary(tokens, lastToken, RawTokenType.Symbol);
                    }
                    else
                    {
                        lastToken = NewTokenIfNecessary(tokens, lastToken, RawTokenType.Literal);
                    }
                    lastToken.ValueBuilder.Append(ch);
                }
            }
            return tokens;
        }

        private RawToken NewTokenIfNecessary(List<RawToken> tokens, RawToken lastToken, RawTokenType curTokenType)
        {
            if (lastToken == null || lastToken.Type != curTokenType)
            {
                lastToken = new RawToken(curTokenType);
                tokens.Add(lastToken);
            }
            return lastToken;
        }
    }

    /// <summary>
    /// A token split from the initial text input.
    /// </summary>
    public class RawToken
    {
        internal RawToken(RawTokenType type)
        {
            Type = type;
            ValueBuilder = new StringBuilder();
        }

        /// <summary>
        /// Gets the token type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public RawTokenType Type { get; private set; }
        internal StringBuilder ValueBuilder { get; private set; }

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
    /// Indicates the raw token type.
    /// </summary>
    public enum RawTokenType
    {
        /// <summary>
        /// Invalid token type.
        /// </summary>
        None,
        /// <summary>
        /// Token is whitespace.
        /// </summary>
        Whitespace,
        /// <summary>
        /// Token is a symbol.
        /// </summary>
        Symbol,
        /// <summary>
        /// Token is not symbol or whitespace.
        /// </summary>
        Literal,
    }
}
