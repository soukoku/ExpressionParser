using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// Defines a basic function routine.
    /// </summary>
    public class FunctionRoutine
    {
        Func<EvaluationContext, ExpressionToken[], ExpressionToken> _routine;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionRoutine"/> class.
        /// </summary>
        /// <param name="argCount">The argument count.</param>
        /// <param name="routine">The routine.</param>
        /// <exception cref="System.ArgumentNullException">routine</exception>
        public FunctionRoutine(int argCount, Func<EvaluationContext, ExpressionToken[], ExpressionToken> routine)
        {
            if (routine == null) { throw new ArgumentNullException("routine"); }
            ArgumentCount = argCount;
            _routine = routine;
        }

        /// <summary>
        /// Gets the expected argument count.
        /// </summary>
        /// <value>
        /// The argument count.
        /// </value>
        public int ArgumentCount { get; private set; }

        /// <summary>
        /// Evaluates using the function routine.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public ExpressionToken Evaluate(EvaluationContext context, ExpressionToken[] args) { return _routine(context, args); }
    }
}
