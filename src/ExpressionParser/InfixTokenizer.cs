using Soukoku.ExpressionParser.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// A tokenizer that parses an input expression string in infix notation into tokens without white spaces.
    /// </summary>
    public class InfixTokenizer
    {
        List<ExpressionToken> _currentTokens;

        /// <summary>
        /// Splits the specified input into a list of <see cref="ExpressionToken" /> values.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public ExpressionToken[] Tokenize(string input)
        {
            _currentTokens = new List<ExpressionToken>();
            ExpressionToken lastExpToken = null;

            var reader = new ListReader<RawToken>(new RawTokenizer().Tokenize(input));

            while (!reader.IsEnd)
            {
                var curRawToken = reader.Read();
                switch (curRawToken.TokenType)
                {
                    case RawTokenType.WhiteSpace:
                        // generially ends previous token outside other special scopes
                        lastExpToken = null;
                        break;
                    case RawTokenType.Literal:
                        if (lastExpToken == null || lastExpToken.TokenType != ExpressionTokenType.Value)
                        {
                            lastExpToken = new ExpressionToken { TokenType = ExpressionTokenType.Value };
                            _currentTokens.Add(lastExpToken);
                        }
                        lastExpToken.Append(curRawToken);
                        break;
                    case RawTokenType.Symbol:
                        // first do operator match by checking the prev op
                        // and see if combined with current token would still match a known operator
                        if (KnownOperators.IsKnown(curRawToken.Value))
                        {
                            if (lastExpToken != null && lastExpToken.TokenType == ExpressionTokenType.Operator)
                            {
                                var testOpValue = lastExpToken.Value + curRawToken.Value;
                                if (KnownOperators.IsKnown(testOpValue))
                                {
                                    // just append it
                                    lastExpToken.Append(curRawToken);
                                    continue;
                                }
                            }
                            // start new one
                            lastExpToken = new ExpressionToken { TokenType = ExpressionTokenType.Operator };
                            _currentTokens.Add(lastExpToken);
                            lastExpToken.Append(curRawToken);
                        }
                        else
                        {
                            lastExpToken = HandleNonOperatorSymbolToken(reader, lastExpToken, curRawToken);
                        }
                        break;
                    default:
                        // should never happen
                        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Unsupported token type {0} at position {1}.", curRawToken.TokenType, curRawToken.Position));
                }
            }

            MassageTokens(_currentTokens);

            return _currentTokens.ToArray();
        }

        ExpressionToken HandleNonOperatorSymbolToken(ListReader<RawToken> reader, ExpressionToken lastExpToken, RawToken curRawToken)
        {
            switch (curRawToken.Value)
            {
                case ",":
                    lastExpToken = new ExpressionToken { TokenType = ExpressionTokenType.Comma };
                    _currentTokens.Add(lastExpToken);
                    lastExpToken.Append(curRawToken);
                    break;
                case "(":
                    // if last one is string make it a function
                    if (lastExpToken != null && lastExpToken.TokenType == ExpressionTokenType.Value)
                    {
                        lastExpToken.TokenType = ExpressionTokenType.Function;
                    }

                    lastExpToken = new ExpressionToken { TokenType = ExpressionTokenType.OpenParenthesis };
                    _currentTokens.Add(lastExpToken);
                    lastExpToken.Append(curRawToken);
                    break;
                case ")":
                    lastExpToken = new ExpressionToken { TokenType = ExpressionTokenType.CloseParenthesis };
                    _currentTokens.Add(lastExpToken);
                    lastExpToken.Append(curRawToken);
                    break;
                case "{":
                    // read until end of }
                    lastExpToken = ReadToLiteralAs(reader, "}", ExpressionTokenType.Field);
                    break;
                case "\"":
                    // read until end of "
                    lastExpToken = ReadToLiteralAs(reader, "\"", ExpressionTokenType.DoubleQuoted);
                    break;
                case "'":
                    // read until end of '
                    lastExpToken = ReadToLiteralAs(reader, "'", ExpressionTokenType.SingleQuoted);
                    break;
            }

            return lastExpToken;
        }

        ExpressionToken ReadToLiteralAs(ListReader<RawToken> reader, string literalValue, ExpressionTokenType tokenType)
        {
            ExpressionToken lastExpToken = new ExpressionToken { TokenType = tokenType };
            _currentTokens.Add(lastExpToken);
            while (!reader.IsEnd)
            {
                var next = reader.Read();
                if (next.TokenType == RawTokenType.Symbol && next.Value == literalValue)
                {
                    break;
                }
                lastExpToken.Append(next);
            }

            return lastExpToken;
        }

        static void MassageTokens(List<ExpressionToken> tokens)
        {
            // change token type based on detected stuff
            var reader = new ListReader<ExpressionToken>(tokens);
            while (!reader.IsEnd)
            {
                var tk = reader.Read();
                tk.Freeze();

                if (tk.TokenType == ExpressionTokenType.Operator)
                {
                    // special detection for operators depending on where it is :(
                    switch (tk.Value)
                    {
                        // TODO: detect post ++ -- versions
                        case "++":
                            tk.OperatorType = OperatorType.PreDecrement;
                            break;
                        case "--":
                            tk.OperatorType = OperatorType.PreDecrement;
                            break;
                        case "+=":
                            tk.OperatorType = OperatorType.AdditionAssignment;
                            break;
                        case "-=":
                            tk.OperatorType = OperatorType.SubtractionAssignment;
                            break;
                        case "*=":
                            tk.OperatorType = OperatorType.MultiplicationAssignment;
                            break;
                        case "/=":
                            tk.OperatorType = OperatorType.DivisionAssignment;
                            break;
                        case "%=":
                            tk.OperatorType = OperatorType.ModulusAssignment;
                            break;
                        case "==":
                            tk.OperatorType = OperatorType.Equal;
                            break;
                        case "!=":
                            tk.OperatorType = OperatorType.NotEqual;
                            break;
                        case "<=":
                            tk.OperatorType = OperatorType.LessThanOrEqual;
                            break;
                        case ">=":
                            tk.OperatorType = OperatorType.GreaterThanOrEqual;
                            break;
                        case "&&":
                            tk.OperatorType = OperatorType.LogicalAnd;
                            break;
                        case "||":
                            tk.OperatorType = OperatorType.LogicalOr;
                            break;

                            // TODO: detect unary versions of + -
                        case "+":
                            tk.OperatorType = OperatorType.Addition;
                            break;
                        case "-":
                            tk.OperatorType = OperatorType.Subtraction;
                            break;
                        case "*":
                            tk.OperatorType = OperatorType.Multiplication;
                            break;
                        case "/":
                            tk.OperatorType = OperatorType.Division;
                            break;
                        case "=":
                            tk.OperatorType = OperatorType.Assignment;
                            break;
                        case "%":
                            tk.OperatorType = OperatorType.Modulus;
                            break;
                        //case "^": tk.OperatorType = OperatorType.
                        case "<":
                            tk.OperatorType = OperatorType.LessThan;
                            break;
                        case ">":
                            tk.OperatorType = OperatorType.GreaterThan;
                            break;
                        //case "~": tk.OperatorType = OperatorType.
                        //case "&": tk.OperatorType = OperatorType.
                        //case "|": tk.OperatorType = OperatorType.
                        case "!":
                            tk.OperatorType = OperatorType.Negation;
                            break;
                    }
                }
            }
        }
    }

}
