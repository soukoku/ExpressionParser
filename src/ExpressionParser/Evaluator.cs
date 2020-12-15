﻿using Soukoku.ExpressionParser.Parsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// An expression evaluator.
    /// </summary>
    public class Evaluator
    {
        EvaluationContext _context;
        Stack<ExpressionToken> _stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="Evaluator"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public Evaluator(EvaluationContext context)
        {
            _context = context ?? throw new ArgumentNullException("context");
        }

        /// <summary>
        /// Evaluates the specified input expression.
        /// </summary>
        /// <param name="input">The input expression (infix).</param>
        /// <param name="coerseToBoolean">if set to <c>true</c> then the result will be coersed to boolean true/false if possible.
        /// Anything not "false", "0", or "" is considered true.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Unbalanced expression.</exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public ExpressionToken Evaluate(string input, bool coerseToBoolean = false)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return coerseToBoolean ? ExpressionToken.False : new ExpressionToken(input);
            }

            var tokens = new InfixToPostfixTokenizer().Tokenize(input);
            // resolve field value and type hints here
            foreach (var token in tokens.Where(tk => tk.TokenType == ExpressionTokenType.Field))
            {
                token.FieldValue = _context.ResolveFieldValue(token.Value);
            }

            var reader = new ListReader<ExpressionToken>(tokens);

            // from https://en.wikipedia.org/wiki/Reverse_Polish_notation
            _stack = new Stack<ExpressionToken>();
            while (!reader.IsEnd)
            {
                var tk = reader.Read();
                switch (tk.TokenType)
                {
                    case ExpressionTokenType.Value:
                    case ExpressionTokenType.DoubleQuoted:
                    case ExpressionTokenType.SingleQuoted:
                    case ExpressionTokenType.Field:
                        _stack.Push(tk);
                        break;
                    case ExpressionTokenType.Operator:
                        HandleOperator(tk.OperatorType);
                        break;
                    case ExpressionTokenType.Function:
                        HandleFunction(tk.Value);
                        break;
                }
            }

            if (_stack.Count == 1)
            {
                var res = _stack.Pop();
                if (coerseToBoolean)
                {
                    if (IsFalse(res.Value))
                    {
                        return ExpressionToken.False;
                    }
                    return ExpressionToken.True;
                }
                return res;

                //if (res.IsNumeric())
                //{
                //    return res;
                //}
                //else if (IsTrue(res.Value))
                //{
                //    return ExpressionToken.True;
                //}
            }
            throw new NotSupportedException("Unbalanced expression.");
        }

        private void HandleFunction(string functionName)
        {
            var fun = _context.GetFunction(functionName);
            var args = new Stack<ExpressionToken>(fun.ArgumentCount);

            while (args.Count < fun.ArgumentCount)
            {
                args.Push(_stack.Pop());
            }

            _stack.Push(fun.Evaluate(_context, args.ToArray()));
        }

        #region operator handling

        static bool IsDate(string lhs, string rhs, out DateTime lhsDate, out DateTime rhsDate)
        {
            lhsDate = default(DateTime);
            rhsDate = default(DateTime);

            if (DateTime.TryParse(lhs, out lhsDate))
            {
                DateTime.TryParse(rhs, out rhsDate);
                return true;
            }
            else if (DateTime.TryParse(rhs, out rhsDate))
            {
                DateTime.TryParse(lhs, out lhsDate);
                return true;
            }
            return false;
        }
        static bool IsNumber(string lhs, string rhs, out decimal lhsNumber, out decimal rhsNumber)
        {
            lhsNumber = 0;
            rhsNumber = 0;

            var islNum = decimal.TryParse(lhs, ExpressionToken.NumberParseStyle, CultureInfo.InvariantCulture, out lhsNumber);
            var isrNum = decimal.TryParse(rhs, ExpressionToken.NumberParseStyle, CultureInfo.InvariantCulture, out rhsNumber);

            return islNum && isrNum;
        }
        static bool IsBoolean(string lhs, string rhs, out bool lhsBool, out bool rhsBool)
        {
            bool lIsBool = false;
            bool rIsBool = false;
            lhsBool = false;
            rhsBool = false;

            if (!string.IsNullOrEmpty(lhs))
            {
                if (string.Equals(lhs, "true", StringComparison.OrdinalIgnoreCase) || lhs == "1")
                {
                    lhsBool = true;
                    lIsBool = true;
                }
                else if (string.Equals(lhs, "false", StringComparison.OrdinalIgnoreCase) || lhs == "0")
                {
                    lIsBool = true;
                }
            }

            if (lIsBool && !string.IsNullOrEmpty(rhs))
            {
                if (string.Equals(rhs, "true", StringComparison.OrdinalIgnoreCase) || rhs == "1")
                {
                    rhsBool = true;
                    rIsBool = true;
                }
                else if (string.Equals(rhs, "false", StringComparison.OrdinalIgnoreCase) || rhs == "0")
                {
                    rIsBool = true;
                }
            }
            return lIsBool && rIsBool;

            //lhsBool = false;
            //rhsBool = false;

            //if (string.Equals(lhs, "true", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(rhs))
            //{
            //    lhsBool = true;
            //    rhsBool = IsTrue(rhs);
            //    return true;
            //}
            //else if (string.Equals(lhs, "false", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(rhs))
            //{
            //    rhsBool = IsTrue(rhs);
            //    return true;
            //}
            //else if (string.Equals(rhs, "true", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(lhs))
            //{
            //    rhsBool = true;
            //    lhsBool = IsTrue(lhs);
            //    return true;
            //}
            //else if (string.Equals(rhs, "false", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(lhs))
            //{
            //    lhsBool = IsTrue(lhs);
            //    return true;
            //}
            //return false;
        }

        private void HandleOperator(OperatorType op)
        {
            switch (op)
            {
                case OperatorType.Addition:
                    BinaryNumberOperation((a, b) => a + b);
                    break;
                case OperatorType.Subtraction:
                    BinaryNumberOperation((a, b) => a - b);
                    break;
                case OperatorType.Multiplication:
                    BinaryNumberOperation((a, b) => a * b);
                    break;
                case OperatorType.Division:
                    BinaryNumberOperation((a, b) => a / b);
                    break;
                case OperatorType.Modulus:
                    BinaryNumberOperation((a, b) => a % b);
                    break;
                // these logical comparision can be date/num/string!
                case OperatorType.LessThan:
                    var rhsToken = _stack.Pop();
                    var lhsToken = _stack.Pop();
                    var rhs = rhsToken.ToString();
                    var lhs = lhsToken.ToString();

                    if (IsNumber(lhs, rhs, out decimal lhsNum, out decimal rhsNum))
                    {
                        _stack.Push(lhsNum < rhsNum ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else if (IsDate(lhs, rhs, out DateTime lhsDate, out DateTime rhsDate))
                    {
                        _stack.Push(lhsDate < rhsDate ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else
                    {
                        _stack.Push(string.Compare(lhs, rhs, StringComparison.OrdinalIgnoreCase) < 0 ? ExpressionToken.True : ExpressionToken.False);
                    }
                    break;
                case OperatorType.LessThanOrEqual:
                    rhsToken = _stack.Pop();
                    lhsToken = _stack.Pop();
                    rhs = rhsToken.ToString();
                    lhs = lhsToken.ToString();

                    if (IsNumber(lhs, rhs, out lhsNum, out rhsNum))
                    {
                        _stack.Push(lhsNum <= rhsNum ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else if (IsDate(lhs, rhs, out DateTime lhsDate, out DateTime rhsDate))
                    {
                        _stack.Push(lhsDate <= rhsDate ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else
                    {
                        _stack.Push(string.Compare(lhs, rhs, StringComparison.OrdinalIgnoreCase) <= 0 ? ExpressionToken.True : ExpressionToken.False);
                    }
                    break;
                case OperatorType.GreaterThan:
                    rhsToken = _stack.Pop();
                    lhsToken = _stack.Pop();
                    rhs = rhsToken.ToString();
                    lhs = lhsToken.ToString();

                    if (IsNumber(lhs, rhs, out lhsNum, out rhsNum))
                    {
                        _stack.Push(lhsNum > rhsNum ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else if (IsDate(lhs, rhs, out DateTime lhsDate, out DateTime rhsDate))
                    {
                        _stack.Push(lhsDate > rhsDate ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else
                    {
                        _stack.Push(string.Compare(lhs, rhs, StringComparison.OrdinalIgnoreCase) > 0 ? ExpressionToken.True : ExpressionToken.False);
                    }
                    break;
                case OperatorType.GreaterThanOrEqual:
                    rhsToken = _stack.Pop();
                    lhsToken = _stack.Pop();
                    rhs = rhsToken.ToString();
                    lhs = lhsToken.ToString();

                    if (IsNumber(lhs, rhs, out lhsNum, out rhsNum))
                    {
                        _stack.Push(lhsNum >= rhsNum ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else if (IsDate(lhs, rhs, out DateTime lhsDate, out DateTime rhsDate))
                    {
                        _stack.Push(lhsDate >= rhsDate ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else
                    {
                        _stack.Push(string.Compare(lhs, rhs, StringComparison.OrdinalIgnoreCase) >= 0 ? ExpressionToken.True : ExpressionToken.False);
                    }
                    break;
                case OperatorType.Equal:
                    rhsToken = _stack.Pop();
                    lhsToken = _stack.Pop();
                    rhs = rhsToken.ToString();
                    lhs = lhsToken.ToString();

                    if (IsBoolean(lhs, rhs, out bool lhsBool, out bool rhsBool))
                    {
                        _stack.Push(lhsBool == rhsBool ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else if ((AllowAutoFormat(lhsToken) || AllowAutoFormat(rhsToken)) &&
                        IsNumber(lhs, rhs, out lhsNum, out rhsNum))
                    {
                        _stack.Push(lhsNum == rhsNum ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else if (IsDate(lhs, rhs, out DateTime lhsDate, out DateTime rhsDate))
                    {
                        _stack.Push(lhsDate == rhsDate ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else
                    {
                        _stack.Push(string.Compare(lhs, rhs, StringComparison.OrdinalIgnoreCase) == 0 ? ExpressionToken.True : ExpressionToken.False);
                    }
                    break;
                case OperatorType.NotEqual:
                    rhsToken = _stack.Pop();
                    lhsToken = _stack.Pop();
                    rhs = rhsToken.ToString();
                    lhs = lhsToken.ToString();

                    if (IsBoolean(lhs, rhs, out lhsBool, out rhsBool))
                    {
                        _stack.Push(lhsBool != rhsBool ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else if ((AllowAutoFormat(lhsToken) || AllowAutoFormat(rhsToken)) &&
                        IsNumber(lhs, rhs, out lhsNum, out rhsNum))
                    {
                        _stack.Push(lhsNum != rhsNum ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else if (IsDate(lhs, rhs, out DateTime lhsDate, out DateTime rhsDate))
                    {
                        _stack.Push(lhsDate != rhsDate ? ExpressionToken.True : ExpressionToken.False);
                    }
                    else
                    {
                        _stack.Push(string.Compare(lhs, rhs, StringComparison.OrdinalIgnoreCase) != 0 ? ExpressionToken.True : ExpressionToken.False);
                    }
                    break;
                case OperatorType.BitwiseAnd:
                    BinaryNumberOperation((a, b) => (int)a & (int)b);
                    break;
                case OperatorType.BitwiseOr:
                    BinaryNumberOperation((a, b) => (int)a | (int)b);
                    break;
                case OperatorType.LogicalAnd:
                    BinaryLogicOperation((a, b) => IsTrue(a) && IsTrue(b));
                    break;
                case OperatorType.LogicalOr:
                    BinaryLogicOperation((a, b) => IsTrue(a) || IsTrue(b));
                    break;
                case OperatorType.UnaryMinus:
                    UnaryNumberOperation(a => -1 * a);
                    break;
                case OperatorType.UnaryPlus:
                    // no action
                    break;
                case OperatorType.LogicalNegation:
                    UnaryLogicOperation(a => !IsTrue(a));
                    break;
                case OperatorType.PreIncrement:
                    UnaryNumberOperation(a => a + 1);
                    break;
                case OperatorType.PreDecrement:
                    UnaryNumberOperation(a => a - 1);
                    break;
                // TODO: handle assignments & post increments
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The {0} operation is not currently supported.", op));
            }
        }

        static bool AllowAutoFormat(ExpressionToken token)
        {
            return token.TokenType != ExpressionTokenType.Field || token.FieldValue.TypeHint != ValueTypeHint.Text;
        }

        static bool IsTrue(string value)
        {
            return string.Equals("true", value, StringComparison.OrdinalIgnoreCase) || value == "1";
        }

        static bool IsFalse(string value)
        {
            return string.Equals("false", value, StringComparison.OrdinalIgnoreCase) || value == "0" || string.IsNullOrWhiteSpace(value);
        }

        void UnaryNumberOperation(Func<decimal, decimal> operation)
        {
            var op1 = _stack.Pop().ToDecimal(_context);
            var res = operation(op1);

            _stack.Push(new ExpressionToken(res.ToString(CultureInfo.InvariantCulture)));
        }
        void UnaryLogicOperation(Func<string, bool> operation)
        {
            var op1 = _stack.Pop();
            var res = operation(op1.ToString()) ? "1" : "0";

            _stack.Push(new ExpressionToken(res));
        }
        void BinaryLogicOperation(Func<string, string, bool> operation)
        {
            var op2 = _stack.Pop();
            var op1 = _stack.Pop();

            var res = operation(op1.ToString(), op2.ToString()) ? "1" : "0";

            _stack.Push(new ExpressionToken(res));
        }
        void BinaryNumberOperation(Func<decimal, decimal, decimal> operation)
        {
            var op2 = _stack.Pop().ToDecimal(_context);
            var op1 = _stack.Pop().ToDecimal(_context);

            var res = operation(op1, op2);

            _stack.Push(new ExpressionToken(res.ToString(CultureInfo.InvariantCulture)));
        }

        #endregion
    }
}
