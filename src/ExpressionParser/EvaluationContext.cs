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
        static Dictionary<string, FunctionRoutine> BuiltInFunctions = new Dictionary<string, FunctionRoutine>(StringComparer.OrdinalIgnoreCase)
        {
            { "pow", new FunctionRoutine(2, (ctx, args)=>
                    new ExpressionToken( Math.Pow(args[0].ToDouble(ctx), args[1].ToDouble(ctx)).ToString(ctx.FormatCulture))) },
            { "sin", new FunctionRoutine(1, (ctx, args)=>
                    new ExpressionToken( Math.Sin(args[0].ToDouble(ctx)).ToString(ctx.FormatCulture)))},
            { "cos", new FunctionRoutine(1, (ctx, args)=>
                    new ExpressionToken( Math.Cos(args[0].ToDouble(ctx)).ToString(ctx.FormatCulture)))},
            { "tan", new FunctionRoutine(1, (ctx, args)=>
                    new ExpressionToken( Math.Tan(args[0].ToDouble(ctx)).ToString(ctx.FormatCulture)))}
        };

        static readonly Dictionary<string, FunctionRoutine> __staticFuncs = new Dictionary<string, FunctionRoutine>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, FunctionRoutine> _instanceFuncs = new Dictionary<string, FunctionRoutine>(StringComparer.OrdinalIgnoreCase);

        Func<string, (object, ValueTypeHint)> _fieldLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationContext"/> class.
        /// </summary>
        public EvaluationContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationContext"/> class.
        /// </summary>
        /// <param name="fieldLookupRoutine">The field value lookup routine.</param>
        public EvaluationContext(Func<string, (object Value, ValueTypeHint TypeHint)> fieldLookupRoutine)
        {
            _fieldLookup = fieldLookupRoutine;
        }

        /// <summary>
        /// Resolves the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public (object Value, ValueTypeHint TypeHint) ResolveFieldValue(string field)
        {
            if (_fieldLookup != null) { return _fieldLookup(field); }
            return OnResolveFieldValue(field);
        }

        readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private CultureInfo _formatCulture = null;

        /// <summary>
        /// Gets/sets the culture used to parse/format expressions. 
        /// Defaults to en-US for certain reasons.
        /// </summary>
        public CultureInfo FormatCulture
        {
            get { return _formatCulture ?? _usCulture; }
            set { _formatCulture = value; }
        }


        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        protected virtual (object Value, ValueTypeHint TypeHint) OnResolveFieldValue(string field)
        {
            return (string.Empty, ValueTypeHint.Auto);
        }

        /// <summary>
        /// Registers a custom function globally.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="info">The information.</param>
        public static void RegisterGlobalFunction(string functionName, FunctionRoutine info)
        {
            __staticFuncs[functionName] = info;
        }

        /// <summary>
        /// Registers a custom function with this context instance.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="info">The information.</param>
        public void RegisterFunction(string functionName, FunctionRoutine info)
        {
            _instanceFuncs[functionName] = info;
        }

        /// <summary>
        /// Gets the function registered with this context.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public FunctionRoutine GetFunction(string functionName)
        {
            if (_instanceFuncs.ContainsKey(functionName))
            {
                return _instanceFuncs[functionName];
            }
            if (__staticFuncs.ContainsKey(functionName))
            {
                return __staticFuncs[functionName];
            }
            if (BuiltInFunctions.ContainsKey(functionName))
            {
                return BuiltInFunctions[functionName];
            }
            return OnGetFunction(functionName) ??
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Function \"{0}\" is not supported.", functionName));
        }


        /// <summary>
        /// Gets the function registered with this context.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected virtual FunctionRoutine OnGetFunction(string functionName)
        {
            return null;
        }
    }
}
