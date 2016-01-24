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
        public decimal EvaluateInfix(string input)
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
                        HandleOperator(tk.OperatorType, reader);
                        break;
                    case ExpressionTokenType.Function:
                        HandleFunction(tk.OperatorType, reader);
                        break;
                }
            }

            if (_stack.Count > 1)
            {
                throw new NotSupportedException("Unbalanced expression.");
            }
            else if (_stack.Count == 1)
            {
                return _context.ToDecimal(_stack.Pop());
            }
            return 0;
        }

        private void HandleOperator(OperatorType op, ListReader<ExpressionToken> reader)
        {
            switch (op)
            {
                case OperatorType.Addition:
                    DoBinaryOperation((a, b) => a + b);
                    break;
                case OperatorType.Subtraction:
                    DoBinaryOperation((a, b) => a - b);
                    break;
                case OperatorType.Multiplication:
                    DoBinaryOperation((a, b) => a * b);
                    break;
                case OperatorType.Division:
                    DoBinaryOperation((a, b) => a / b);
                    break;
                case OperatorType.Modulus:
                    DoBinaryOperation((a, b) => a % b);
                    break;
                //TODO: these logical comparision are very badly implemented
                case OperatorType.LessThan:
                    DoBinaryOperation((a, b) => a < b ? 1 : 0);
                    break;
                case OperatorType.LessThanOrEqual:
                    DoBinaryOperation((a, b) => a <= b ? 1 : 0);
                    break;
                case OperatorType.GreaterThan:
                    DoBinaryOperation((a, b) => a > b ? 1 : 0);
                    break;
                case OperatorType.GreaterThanOrEqual:
                    DoBinaryOperation((a, b) => a >= b ? 1 : 0);
                    break;
                case OperatorType.Equal:
                    DoBinaryOperation((a, b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase));
                    break;
                case OperatorType.NotEqual:
                    DoBinaryOperation((a, b) => !string.Equals(a, b, StringComparison.OrdinalIgnoreCase));
                    break;
                case OperatorType.BitwiseAnd:
                    DoBinaryOperation((a, b) => (int)a & (int)b);
                    break;
                case OperatorType.BitwiseOr:
                    DoBinaryOperation((a, b) => (int)a | (int)b);
                    break;
                case OperatorType.LogicalAnd:
                    DoBinaryOperation((a, b) => ToBool(a) && ToBool(b));
                    break;
                case OperatorType.LogicalOr:
                    DoBinaryOperation((a, b) => ToBool(a) || ToBool(b));
                    break;
                // TODO: assignments & unary ops
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The {0} operation is not currently supported.", op));
            }
        }

        private bool ToBool(string value)
        {
            return string.Equals("true", value) || value == "1";
        }

        void DoBinaryOperation(Func<string, string, bool> operation)
        {
            var op2 = _stack.Pop();
            var op1 = _stack.Pop();

            var val = operation(op1.Value, op2.Value) ? "1" : "0";

            _stack.Push(new ExpressionToken(val));
        }
        void DoBinaryOperation(Func<decimal, decimal, decimal> operation)
        {
            var op2 = _context.ToDecimal(_stack.Pop());
            var op1 = _context.ToDecimal(_stack.Pop());

            _stack.Push(new ExpressionToken(operation(op1, op2).ToString()));
        }

        private void HandleFunction(OperatorType op, ListReader<ExpressionToken> reader)
        {
            throw new NotImplementedException();
        }
    }
}
