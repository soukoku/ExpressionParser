using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser.Parsing
{
    /// <summary>
    /// Indicates the recognized operator types.
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// Unspecified default value.
        /// </summary>
        None,
        /// <summary>
        /// ++ after a value.
        /// </summary>
        PostIncrement,
        /// <summary>
        /// -- after a value.
        /// </summary>
        PostDecrement,
        /// <summary>
        /// ++ before a value.
        /// </summary>
        PreIncrement,
        /// <summary>
        /// -- before a value.
        /// </summary>
        PreDecrement,
        /// <summary>
        /// + before a value.
        /// </summary>
        UnaryPlus,
        /// <summary>
        /// - before a value.
        /// </summary>
        UnaryMinus,
        /// <summary>
        /// ! before a value.
        /// </summary>
        LogicalNegation,
        /// <summary>
        /// * between values.
        /// </summary>
        Multiplication,
        /// <summary>
        /// / between values.
        /// </summary>
        Division,
        /// <summary>
        /// % between values.
        /// </summary>
        Modulus,
        /// <summary>
        /// + between values.
        /// </summary>
        Addition,
        /// <summary>
        /// - between values.
        /// </summary>
        Subtraction,
        /// <summary>
        /// &lt; between values.
        /// </summary>
        LessThan,
        /// <summary>
        /// &lt;= between values.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// &gt; between values.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// &gt;= between values.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// == between values.
        /// </summary>
        Equal,
        /// <summary>
        /// != between values.
        /// </summary>
        NotEqual,
        /// <summary>
        /// &amp; between values.
        /// </summary>
        BitwiseAnd,
        /// <summary>
        /// | between values.
        /// </summary>
        BitwiseOr,
        /// <summary>
        /// &amp;&amp; between values.
        /// </summary>
        LogicalAnd,
        /// <summary>
        /// || between values.
        /// </summary>
        LogicalOr,
        /// <summary>
        /// = between values.
        /// </summary>
        Assignment,
        /// <summary>
        /// += between values.
        /// </summary>
        AdditionAssignment,
        /// <summary>
        /// -= between values.
        /// </summary>
        SubtractionAssignment,
        /// <summary>
        /// *= between values.
        /// </summary>
        MultiplicationAssignment,
        /// <summary>
        /// /= between values.
        /// </summary>
        DivisionAssignment,
        /// <summary>
        /// %= between values.
        /// </summary>
        ModulusAssignment,

    }
}
