using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// Context for storing and returning values during an expression evaluation.
    /// </summary>
    public class EvaluationContext
    {
        static Dictionary<string, FunctionInfo> BuiltInFunctions = new Dictionary<string, FunctionInfo>
        {
            { "pow", new FunctionInfo(2, (ctx, args)=>
                    new ExpressionToken( Math.Pow(args[0].ToDouble(ctx), args[1].ToDouble(ctx)).ToString())) },
            { "sin", new FunctionInfo(1, (ctx, args)=>
                    new ExpressionToken( Math.Sin(args[0].ToDouble(ctx)).ToString()))},
            { "cos", new FunctionInfo(1, (ctx, args)=>
                    new ExpressionToken( Math.Cos(args[0].ToDouble(ctx)).ToString()))},
            { "tan", new FunctionInfo(1, (ctx, args)=>
                    new ExpressionToken( Math.Tan(args[0].ToDouble(ctx)).ToString()))}
        };

        Dictionary<string, FunctionInfo> _instanceFuncs = new Dictionary<string, FunctionInfo>();

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object GetFieldValue(string field)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers a function with this context.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="info">The information.</param>
        public void RegisterFunction(string functionName, FunctionInfo info)
        {
            _instanceFuncs[functionName] = info;
        }

        /// <summary>
        /// Gets the function registered with this context.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public FunctionInfo GetFunction(string functionName)
        {
            if (_instanceFuncs.ContainsKey(functionName))
            {
                return _instanceFuncs[functionName];
            }
            if (BuiltInFunctions.ContainsKey(functionName))
            {
                return BuiltInFunctions[functionName];
            }
            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Function \"{0}\" is not supported.", functionName));
        }
    }

    /// <summary>
    /// Defines a basic function routine.
    /// </summary>
    public class FunctionInfo
    {
        Func<EvaluationContext, ExpressionToken[], ExpressionToken> _routine;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionInfo"/> class.
        /// </summary>
        /// <param name="argCount">The argument count.</param>
        /// <param name="routine">The routine.</param>
        /// <exception cref="System.ArgumentNullException">routine</exception>
        public FunctionInfo(int argCount, Func<EvaluationContext, ExpressionToken[], ExpressionToken> routine)
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
