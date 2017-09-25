using Soukoku.ExpressionParser.Utilities;
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
            if (context == null) { throw new ArgumentNullException("context"); }
            _context = context;
        }

        /// <summary>
        /// Evaluates the specified input as an infix expression.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public ExpressionToken EvaluateInfix(string input)
        {
            var tokens = new InfixToPostfixTokenizer().Tokenize(input);
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

            if (_stack.Count > 1)
            {
                throw new NotSupportedException("Unbalanced expression.");
            }
            else if (_stack.Count == 1)
            {
                var res = _stack.Pop();
                if (IsNumeric(res.Value))
                {
                    return res;
                }
                else if (IsTrue(res.Value))
                {
                    return new ExpressionToken("1");
                }
                //else if (IsFalse(res.Value))
                //{
                //    return new ExpressionToken("0");
                //}
            }
            return new ExpressionToken("0");
        }

        static bool IsNumeric(string value)
        {
            decimal dummy;
            return decimal.TryParse(value, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out dummy);
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
                //TODO: these logical comparision are likely very badly implemented
                case OperatorType.LessThan:
                    BinaryNumberOperation((a, b) => a < b ? 1 : 0);
                    break;
                case OperatorType.LessThanOrEqual:
                    BinaryNumberOperation((a, b) => a <= b ? 1 : 0);
                    break;
                case OperatorType.GreaterThan:
                    BinaryNumberOperation((a, b) => a > b ? 1 : 0);
                    break;
                case OperatorType.GreaterThanOrEqual:
                    BinaryNumberOperation((a, b) => a >= b ? 1 : 0);
                    break;
                case OperatorType.Equal:
                    BinaryLogicOperation((a, b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase));
                    break;
                case OperatorType.NotEqual:
                    BinaryLogicOperation((a, b) => !string.Equals(a, b, StringComparison.OrdinalIgnoreCase));
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

        static bool IsTrue(string value)
        {
            return string.Equals("true", value, StringComparison.OrdinalIgnoreCase) || value == "1";
        }

        static bool IsFalse(string value)
        {
            return string.Equals("false", value, StringComparison.OrdinalIgnoreCase) || value == "0";
        }

        void UnaryNumberOperation(Func<decimal, decimal> operation)
        {
            var op1 = _stack.Pop().ToDecimal(_context);
            var res = operation(op1);

            _stack.Push(new ExpressionToken(res.ToString(CultureInfo.CurrentCulture)));
        }
        void UnaryLogicOperation(Func<string, bool> operation)
        {
            var op1 = _stack.Pop();
            var res = operation(op1.ToString(_context)) ? "1" : "0";

            _stack.Push(new ExpressionToken(res));
        }
        void BinaryLogicOperation(Func<string, string, bool> operation)
        {
            var op2 = _stack.Pop();
            var op1 = _stack.Pop();

            var res = operation(op1.ToString(_context), op2.ToString(_context)) ? "1" : "0";

            _stack.Push(new ExpressionToken(res));
        }
        void BinaryNumberOperation(Func<decimal, decimal, decimal> operation)
        {
            var op2 = _stack.Pop().ToDecimal(_context);
            var op1 = _stack.Pop().ToDecimal(_context);

            var res = operation(op1, op2);

            _stack.Push(new ExpressionToken(res.ToString(CultureInfo.CurrentCulture)));
        }

        #endregion
    }
}
