using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// A tokenizer that parses an input expression string in infix notation into tokens without white spaces
    /// in the orders of postfix expressions.
    /// </summary>
    public class InfixToPostfixTokenizer : IExpressionTokenizer
    {
        const string UnbalancedParenMsg = "Unbalanced parenthesis in expression.";

        List<ExpressionToken> _output;
        Stack<ExpressionToken> _stack;

        /// <summary>
        /// Splits the specified input into a list of <see cref="ExpressionToken" /> values
        /// in postfix order.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public ExpressionToken[] Tokenize(string input)
        {
            var infixTokens = new InfixTokenizer().Tokenize(input);
            _output = new List<ExpressionToken>();
            _stack = new Stack<ExpressionToken>();

            // this is the shunting-yard algorithm
            // https://en.wikipedia.org/wiki/Shunting-yard_algorithm

            foreach (var inToken in infixTokens)
            {
                switch (inToken.TokenType)
                {
                    case ExpressionTokenType.Value:
                    case ExpressionTokenType.DoubleQuoted:
                    case ExpressionTokenType.SingleQuoted:
                    case ExpressionTokenType.Field:
                        _output.Add(inToken);
                        break;
                    case ExpressionTokenType.Function:
                        _stack.Push(inToken);
                        break;
                    case ExpressionTokenType.Comma:
                        HandleComma();
                        break;
                    case ExpressionTokenType.Operator:
                        HandleOperatorToken(inToken);
                        break;
                    case ExpressionTokenType.OpenParenthesis:
                        _stack.Push(inToken);
                        break;
                    case ExpressionTokenType.CloseParenthesis:
                        HandleCloseParenthesis();
                        break;
                }
            }

            while (_stack.Count > 0)
            {
                var op = _stack.Pop();
                if (op.TokenType == ExpressionTokenType.OpenParenthesis)
                {
                    throw new NotSupportedException(UnbalancedParenMsg);
                }
                _output.Add(op);
            }

            return _output.ToArray();
        }

        private void HandleComma()
        {
            bool closed = false;
            while (_stack.Count > 1)
            {
                var peek = _stack.Peek();
                if (peek.TokenType == ExpressionTokenType.OpenParenthesis)
                {
                    closed = true;
                    break;
                }
                _output.Add(_stack.Pop());
            }

            if (!closed)
            {
                throw new NotSupportedException(UnbalancedParenMsg);
            }
        }

        private void HandleOperatorToken(ExpressionToken inToken)
        {
            while (_stack.Count > 0)
            {
                var op2 = _stack.Peek();
                if (op2.TokenType == ExpressionTokenType.Operator)
                {
                    var op1Prec = KnownOperators.GetPrecedence(inToken.OperatorType);
                    var op2Prec = KnownOperators.GetPrecedence(op2.OperatorType);
                    var op1IsLeft = KnownOperators.IsLeftAssociative(inToken.OperatorType);

                    if ((op1IsLeft && op1Prec <= op2Prec) ||
                        (!op1IsLeft && op1Prec < op2Prec))
                    {
                        _output.Add(_stack.Pop());
                        continue;
                    }
                }
                break;
            }
            _stack.Push(inToken);
        }

        private void HandleCloseParenthesis()
        {
            bool closed = false;
            while (_stack.Count > 0)
            {
                var pop = _stack.Pop();
                if (pop.TokenType == ExpressionTokenType.OpenParenthesis)
                {
                    closed = true;
                    break;
                }
                _output.Add(pop);
            }

            if (!closed)
            {
                throw new NotSupportedException(UnbalancedParenMsg);
            }
        }
    }
}
