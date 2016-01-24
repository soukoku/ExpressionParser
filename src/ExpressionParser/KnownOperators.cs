using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// Contains supported operator info.
    /// </summary>
    public static class KnownOperators
    {
        //static readonly string[] KnownList = new[]
        //{
        //    // double char
        //    "++",
        //    "--",
        //    "+=",
        //    "-=",
        //    "*=",
        //    "/=",
        //    "%=",
        //    "==",
        //    "!=",
        //    "<=",
        //    ">=",
        //    "&&",
        //    "||",

        //    // single char
        //    "+",
        //    "-",
        //    "*",
        //    "/",
        //    "=",
        //    "%", 
        //    //"^",
        //    "<",
        //    ">", 
        //    //"~",
        //    "&",
        //    "|",
        //    "!",
        //};

        static readonly Dictionary<string, OperatorType> DefaultMap = new Dictionary<string, OperatorType>
        {
            // double char
            {"++", OperatorType.PreIncrement },
            {"--", OperatorType.PreDecrement },
            {"+=", OperatorType.AdditionAssignment },
            {"-=", OperatorType.SubtractionAssignment },
            {"*=", OperatorType.MultiplicationAssignment },
            {"/=", OperatorType.DivisionAssignment },
            {"%=", OperatorType.ModulusAssignment },
            {"==", OperatorType.Equal },
            {"!=", OperatorType.NotEqual},
            {"<=", OperatorType.LessThanOrEqual },
            {">=", OperatorType.GreaterThan },
            {"&&", OperatorType.LogicalAnd },
            {"||", OperatorType.LogicalOr },

            // single char
            {"+", OperatorType.Addition },
            {"-", OperatorType.Subtraction },
            {"*", OperatorType.Multiplication },
            {"/", OperatorType.Division },
            {"=", OperatorType.Assignment },
            {"%", OperatorType.Modulus },
            //"^",
            {"<", OperatorType.LessThan },
            {">", OperatorType.GreaterThan },
            //"~",
            {"&", OperatorType.BitwiseAnd },
            {"|", OperatorType.BitwiseOr },
            {"!", OperatorType.Negation },
        };

        public static bool IsKnown(string operatorValue)
        {
            return DefaultMap.ContainsKey(operatorValue);
        }

        public static OperatorType TryMap(string operatorValue)
        {
            if (DefaultMap.ContainsKey(operatorValue))
            {
                return DefaultMap[operatorValue];
            }
            return OperatorType.None;
        }

        public static int GetPrecedence(OperatorType type)
        {
            switch (type)
            {
                case OperatorType.PostDecrement:
                case OperatorType.PostIncrement:
                    return 100;
                case OperatorType.PreDecrement:
                case OperatorType.PreIncrement:
                case OperatorType.UnaryMinus:
                case OperatorType.UnaryPlus:
                case OperatorType.Negation:
                    return 90;
                case OperatorType.Multiplication:
                case OperatorType.Division:
                case OperatorType.Modulus:
                    return 85;
                case OperatorType.Addition:
                case OperatorType.Subtraction:
                    return 80;
                case OperatorType.LessThan:
                case OperatorType.LessThanOrEqual:
                case OperatorType.GreaterThan:
                case OperatorType.GreaterThanOrEqual:
                    return 75;
                case OperatorType.Equal:
                case OperatorType.NotEqual:
                    return 70;
                case OperatorType.BitwiseAnd:
                case OperatorType.BitwiseOr:
                    return 65;
                case OperatorType.LogicalAnd:
                case OperatorType.LogicalOr:
                    return 60;
                case OperatorType.Assignment:
                case OperatorType.AdditionAssignment:
                case OperatorType.DivisionAssignment:
                case OperatorType.ModulusAssignment:
                case OperatorType.MultiplicationAssignment:
                case OperatorType.SubtractionAssignment:
                    return 20;
            }
            return 0;
        }
        public static bool IsLeftAssociative(OperatorType type)
        {
            switch (type)
            {
                case OperatorType.PreDecrement:
                case OperatorType.PreIncrement:
                case OperatorType.UnaryMinus:
                case OperatorType.UnaryPlus:
                case OperatorType.Negation:
                case OperatorType.Assignment:
                case OperatorType.AdditionAssignment:
                case OperatorType.DivisionAssignment:
                case OperatorType.ModulusAssignment:
                case OperatorType.MultiplicationAssignment:
                case OperatorType.SubtractionAssignment:
                    return false;
            }
            return true;
        }
    }

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
        Negation,
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
        /// & between values.
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
