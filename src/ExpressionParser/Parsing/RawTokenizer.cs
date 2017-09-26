using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Soukoku.ExpressionParser.Parsing
{
    /// <summary>
    /// A low-level tokenizer that parses an input expression string into tokens.
    /// </summary>
    public class RawTokenizer
    {
        static readonly char[] DefaultSymbols = new[]
            {
                '+', '-', '*', '/', '=', '%', '^',
                ',', '<', '>', '&', '|', '!',
                '(', ')', '{', '}', '[', ']',
                '"', '\'', '~'
            };


        /// <summary>
        /// Initializes a new instance of the <see cref="RawTokenizer"/> class.
        /// </summary>
        public RawTokenizer() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawTokenizer" /> class.
        /// </summary>
        /// <param name="symbols">The char values to count as symbols. If null the <see cref="DefaultSymbols"/> will be used.</param>
        public RawTokenizer(params char[] symbols)
        {
            _symbols = symbols ?? DefaultSymbols;
        }

        char[] _symbols;

        /// <summary>
        /// Gets the char values that count as symbols for this tokenizer.
        /// </summary>
        /// <value>
        /// The symbols.
        /// </value>
        public char[] GetSymbols() { return (char[])_symbols.Clone(); }

        /// <summary>
        /// Splits the specified input into a list of <see cref="RawToken"/> values using white space and symbols.
        /// The tokens can be recombined to rebuild the original input exactly.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public RawToken[] Tokenize(string input)
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
                        lastToken = NewTokenIfNecessary(tokens, lastToken, RawTokenType.WhiteSpace, i);
                    }
                    else if (_symbols.Contains(ch))
                    {
                        lastToken = NewTokenIfNecessary(tokens, lastToken, RawTokenType.Symbol, i);
                    }
                    else
                    {
                        lastToken = NewTokenIfNecessary(tokens, lastToken, RawTokenType.Literal, i);
                    }

                    if (ch == '\\' && ++i < input.Length)
                    {
                        // assume escape and just append next char as-is
                        var next = input[i];
                        lastToken.ValueBuilder.Append(next);
                    }
                    else
                    {
                        lastToken.ValueBuilder.Append(ch);
                    }
                }
            }
            return tokens.ToArray();
        }

        static RawToken NewTokenIfNecessary(List<RawToken> tokens, RawToken lastToken, RawTokenType curTokenType, int position)
        {
            if (lastToken == null || lastToken.TokenType != curTokenType ||
                curTokenType == RawTokenType.Symbol) // for symbol always let it be by itself
            {
                lastToken = new RawToken(curTokenType, position);
                tokens.Add(lastToken);
            }
            return lastToken;
        }
    }
}
