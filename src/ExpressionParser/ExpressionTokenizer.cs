using Soukoku.ExpressionParser.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// A tokenizer that parses an input expression string into immediate tokens without white spaces.
    /// </summary>
    public class ExpressionTokenizer
    {
        // TODO: add all common code operators

        static readonly Dictionary<string, ExpressionOperator> KnownOperators = new Dictionary<string, ExpressionOperator>
        {
            // double char
            {"++", new ExpressionOperator{ Value= "++" } },
            {"--", new ExpressionOperator{ Value= "++" } },
            //{"+=", new ExpressionOperator{ Value= "+=" } },
            //{"-=", new ExpressionOperator{ Value= "-=" } },
            //{"*=", new ExpressionOperator{ Value= "*=" } },
            //{"/=", new ExpressionOperator{ Value= "/=" } },
            //{"%=", new ExpressionOperator{ Value= "%=" } },
            {"==", new ExpressionOperator{ Value= "==" } },
            {"!=", new ExpressionOperator{ Value= "!=" } },
            {"<=", new ExpressionOperator{ Value= "<=" } },
            {">=", new ExpressionOperator{ Value= ">=" } },
            {"&&", new ExpressionOperator{ Value= "&&" } },
            {"||", new ExpressionOperator{ Value= "||" } },

            // single char
            {"+", new ExpressionOperator{ Value= "+" } },
            {"-", new ExpressionOperator{ Value= "-" } },
            {"*", new ExpressionOperator{ Value= "*" } },
            {"/", new ExpressionOperator{ Value= "/" } },
            {"=", new ExpressionOperator{ Value= "=" } },
            {"%", new ExpressionOperator{ Value= "%" } },
            {"^", new ExpressionOperator{ Value= "^" } },
            {"<", new ExpressionOperator{ Value= "<" } },
            {">", new ExpressionOperator{ Value= ">" } },
            {"~", new ExpressionOperator{ Value= "!" } },
            {"&", new ExpressionOperator{ Value= "&" } },
            {"|", new ExpressionOperator{ Value= "|" } },
            {"!", new ExpressionOperator{ Value= "!" } },
            //new ExpressionOperator{ Value= "[" },
            //new ExpressionOperator{ Value= "]" },
        };


        /// <summary>
        /// Splits the specified input into a list of <see cref="ExpressionToken" /> values.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public ExpressionToken[] Tokenize(string input)
        {
            var tokens = new List<ExpressionToken>();
            ExpressionToken lastToken = null;

            var reader = new ListReader<RawToken>(new RawTokenizer().Tokenize(input));

            while (!reader.IsEol)
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
                        lastToken.Append(rawToken);
                        break;
                    case RawTokenType.Symbol:
                        // first do operator match by checking the prev op
                        // and see if combined with current token would still match a known operator
                        if (KnownOperators.ContainsKey(rawToken.Value))
                        {
                            if (lastToken != null && lastToken.TokenType == ExpressionTokenType.Operator)
                            {
                                var testOpValue = lastToken.Value + rawToken.Value;
                                if (KnownOperators.ContainsKey(testOpValue))
                                {
                                    // just append it
                                    lastToken.Append(rawToken);
                                    continue;
                                }
                            }
                            // start new one
                            lastToken = new ExpressionToken { TokenType = ExpressionTokenType.Operator };
                            tokens.Add(lastToken);
                            lastToken.Append(rawToken);
                        }
                        else
                        {
                            // non-operator symbols

                            switch (rawToken.Value)
                            {
                                case ",":
                                    lastToken = new ExpressionToken { TokenType = ExpressionTokenType.Comma };
                                    tokens.Add(lastToken);
                                    lastToken.Append(rawToken);
                                    break;
                                case "(":
                                    // if last one is string make it a function
                                    if (lastToken != null && lastToken.TokenType == ExpressionTokenType.Value)
                                    {
                                        lastToken.TokenType = ExpressionTokenType.Function;
                                    }

                                    lastToken = new ExpressionToken { TokenType = ExpressionTokenType.OpenParenthesis };
                                    tokens.Add(lastToken);
                                    lastToken.Append(rawToken);
                                    break;
                                case ")":
                                    lastToken = new ExpressionToken { TokenType = ExpressionTokenType.CloseParenthesis };
                                    tokens.Add(lastToken);
                                    lastToken.Append(rawToken);
                                    break;
                                case "{":
                                    // read until end of }
                                    lastToken = new ExpressionToken { TokenType = ExpressionTokenType.Field };
                                    tokens.Add(lastToken);
                                    while (!reader.IsEol)
                                    {
                                        var next = reader.ReadNext();
                                        if (next.TokenType == RawTokenType.Symbol && next.Value == "}")
                                        {
                                            break;
                                        }
                                        lastToken.Append(next);
                                    }
                                    break;
                                case "\"":
                                    // read until end of "
                                    lastToken = new ExpressionToken { TokenType = ExpressionTokenType.DoubleQuoted };
                                    tokens.Add(lastToken);
                                    while (!reader.IsEol)
                                    {
                                        var next = reader.ReadNext();
                                        if (next.TokenType == RawTokenType.Symbol && next.Value == "\"")
                                        {
                                            break;
                                        }
                                        lastToken.Append(next);
                                    }
                                    break;
                                case "'":
                                    // read until end of '
                                    lastToken = new ExpressionToken { TokenType = ExpressionTokenType.SingleQuoted };
                                    tokens.Add(lastToken);
                                    while (!reader.IsEol)
                                    {
                                        var next = reader.ReadNext();
                                        if (next.TokenType == RawTokenType.Symbol && next.Value == "'")
                                        {
                                            break;
                                        }
                                        lastToken.Append(next);
                                    }
                                    break;
                            }
                            //if(rt.Value[0])

                        }
                        break;
                    default:
                        // should never happen
                        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Unsupported token type {0} at position {1}.", rawToken.TokenType, rawToken.Position));
                }
            }

            MassageTokens(tokens);

            return tokens.ToArray();
        }

        private void MassageTokens(List<ExpressionToken> tokens)
        {
            // change token type based on detected stuff
            foreach (var tk in tokens)
            {
                tk.Freeze();
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
        /// <summary>
        /// Gets the operator value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }

    /// <summary>
    /// A token split from the initial text input.
    /// </summary>
    public class ExpressionToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionToken"/> class.
        /// </summary>
        public ExpressionToken()
        {
            _rawTokens = new List<RawToken>();
        }

        IList<RawToken> _rawTokens; // the raw tokens that makes this token

        /// <summary>
        /// Gets the raw tokens that made this list.
        /// </summary>
        /// <returns></returns>
        public RawToken[] GetRawTokens() { return _rawTokens.ToArray(); }

        /// <summary>
        /// Appends the specified token to this expression.
        /// </summary>
        /// <param name="token">The token.</param>
        public void Append(RawToken token)
        {
            _rawTokens.Add(token);
        }

        /// <summary>
        /// Freezes this instance from being appended.
        /// </summary>
        public void Freeze()
        {
            _rawTokens = _rawTokens.ToArray();
            _value = string.Join("", _rawTokens);
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

        string _value;
        /// <summary>
        /// Gets the token value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get { return _value ?? string.Join("", _rawTokens); } }

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
