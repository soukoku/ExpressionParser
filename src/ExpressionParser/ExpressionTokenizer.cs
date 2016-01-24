using Soukoku.ExpressionParser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// A tokenizer that parses an input expression string into immediate tokens without white spaces.
    /// </summary>
    public class ExpressionTokenizer
    {
        static readonly ExpressionOperator[] KnownOperators = new[]
        {
            new ExpressionOperator{ Value= "+" },
            new ExpressionOperator{ Value= "-" },
            new ExpressionOperator{ Value= "*" },
            new ExpressionOperator{ Value= "/" },
            new ExpressionOperator{ Value= "=" },
            new ExpressionOperator{ Value= "%" },
            new ExpressionOperator{ Value= "^" },
            new ExpressionOperator{ Value= "<" },
            new ExpressionOperator{ Value= ">" },
            new ExpressionOperator{ Value= "&" },
            new ExpressionOperator{ Value= "|" },
            new ExpressionOperator{ Value= "!" },
            //new ExpressionOperator{ Value= "[" },
            //new ExpressionOperator{ Value= "]" },
        };


        /// <summary>
        /// Splits the specified input into a list of <see cref="ExpressionToken" /> values.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public IList<ExpressionToken> Tokenize(string input)
        {
            var tokens = new List<ExpressionToken>();

            var reader = new ListReader<RawToken>(new RawTokenizer().Tokenize(input));

            if (!reader.IsEol)
            {
                ExpressionToken lastToken = null;

                do
                {
                    var rawToken = reader.ReadNext();
                    switch (rawToken.TokenType)
                    {
                        case RawTokenType.WhiteSpace:
                            // generially ends previous token outside other special scopes
                            lastToken = null;
                            break;
                        case RawTokenType.Literal:
                            if (lastToken == null || lastToken.TokenType != ExpressionTokenType.Value)
                            {
                                lastToken = new ExpressionToken { TokenType = ExpressionTokenType.Value };
                                tokens.Add(lastToken);
                            }
                            lastToken.Value += rawToken.Value;
                            break;
                        case RawTokenType.Symbol:
                            if (lastToken == null || lastToken.TokenType != ExpressionTokenType.Operator)
                            {
                                lastToken = new ExpressionToken { TokenType = ExpressionTokenType.Operator };
                                tokens.Add(lastToken);
                            }
                            lastToken.Value += rawToken.Value;
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Unsupported token type {0}.", rawToken.TokenType));
                    }
                }
                while (!reader.IsEol);
            }

            RepurposeTokens(tokens);

            return tokens;
        }

        private void RepurposeTokens(List<ExpressionToken> tokens)
        {
            // change token type based on detected stuff
            foreach (var tk in tokens)
            {
                if (tk.TokenType == ExpressionTokenType.Value)
                {

                }
            }
        }
    }

    /// <summary>
    /// Info on an operator.
    /// </summary>
    public class ExpressionOperator
    {
        public string Value { get; set; }
    }

    /// <summary>
    /// A token split from the initial text input.
    /// </summary>
    public class ExpressionToken
    {
        public ExpressionToken()
        {
            Value = "";
        }
        public ExpressionTokenType TokenType { get; set; }

        public string Value { get; set; }

    }

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
        OpenParen,
        /// <summary>
        /// The token is a close parenthesis.
        /// </summary>
        CloseParen,
        /// <summary>
        /// The token is a double quote.
        /// </summary>
        DoubleQuote,
        /// <summary>
        /// The token is a single quote.
        /// </summary>
        SingleQuote,
        ///// <summary>
        ///// The token is a function.
        ///// </summary>
        //Function,
        /// <summary>
        /// The token is a yet-to-be-parsed value.
        /// </summary>
        Value,
    }
}
