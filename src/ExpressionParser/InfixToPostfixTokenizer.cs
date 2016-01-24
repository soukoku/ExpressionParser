using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// A tokenizer that parses an input expression string in infix notation into tokens without white spaces
    /// in the orders of postfix expressions.
    /// </summary>
    public class InfixToPostfixTokenizer
    {
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

            // this is the shunting-yard algorithm
            // https://en.wikipedia.org/wiki/Shunting-yard_algorithm
            List<ExpressionToken> output = new List<ExpressionToken>();
            Stack<ExpressionToken> stack = new Stack<ExpressionToken>();

            foreach (var inToken in infixTokens)
            {
                switch (inToken.TokenType)
                {
                    case ExpressionTokenType.Value:
                    case ExpressionTokenType.DoubleQuoted:
                    case ExpressionTokenType.SingleQuoted:
                    case ExpressionTokenType.Field:
                        output.Add(inToken);
                        break;
                    case ExpressionTokenType.Function:
                        stack.Push(inToken);
                        break;
                    case ExpressionTokenType.Comma:
                        while (stack.Count > 1)
                        {
                            var peek = stack.Peek();
                            if (peek.TokenType == ExpressionTokenType.OpenParenthesis)
                            {
                                break;
                            }
                            output.Add(stack.Pop());
                        }
                        break;
                    case ExpressionTokenType.Operator:
                        var op1 = inToken;
                        while (stack.Count > 0)
                        {
                            var op2 = stack.Peek();
                            if (op2.TokenType == ExpressionTokenType.Operator)
                            {
                                var op1Prec = KnownOperators.GetPrecedence(op1.OperatorType);
                                var op2Prec = KnownOperators.GetPrecedence(op2.OperatorType);
                                var op1IsLeft = KnownOperators.IsLeftAssociative(op1.OperatorType);
                                var op2IsLeft = KnownOperators.IsLeftAssociative(op2.OperatorType);

                                if ((op1IsLeft && op1Prec <= op2Prec) ||
                                    (!op1IsLeft && op1Prec < op2Prec))
                                {
                                    output.Add(stack.Pop());
                                    continue;
                                }
                            }
                            break;
                        }
                        stack.Push(op1);
                        break;
                    case ExpressionTokenType.OpenParenthesis:
                        stack.Push(inToken);
                        break;
                    case ExpressionTokenType.CloseParenthesis:
                        while (stack.Count > 0)
                        {
                            var pop = stack.Pop();
                            if (pop.TokenType == ExpressionTokenType.OpenParenthesis)
                            {
                                break;
                            }
                            output.Add(pop);
                        }
                        break;
                }
            }

            while (stack.Count > 0)
            {
                var op = stack.Pop();
                if (op.TokenType == ExpressionTokenType.OpenParenthesis)
                {
                    // TODO: throw
                }
                output.Add(op);
            }

            return output.ToArray();
        }
    }
}
