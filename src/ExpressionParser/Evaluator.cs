using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Evaluator"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public Evaluator(EvaluationContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            _context = null;
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

            return 0;
        }


    }
}
