using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser.Parsing
{
    /// <summary>
    /// Contains recognized operator info.
    /// </summary>
    public static class KnownOperators
    {
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
            {">=", OperatorType.GreaterThanOrEqual },
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
            {"!", OperatorType.LogicalNegation },
        };

        /// <summary>
        /// Determines whether the specified operator value is recognized.
        /// </summary>
        /// <param name="operatorValue">The operator value.</param>
        /// <returns></returns>
        public static bool IsKnown(string operatorValue)
        {
            return DefaultMap.ContainsKey(operatorValue);
        }

        /// <summary>
        /// Try to get the enum version of the operator string value.
        /// </summary>
        /// <param name="operatorValue">The operator value.</param>
        /// <returns></returns>
        public static OperatorType TryMap(string operatorValue)
        {
            if (DefaultMap.ContainsKey(operatorValue))
            {
                return DefaultMap[operatorValue];
            }
            return OperatorType.None;
        }

        /// <summary>
        /// Gets the precedence of an operator.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
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
                case OperatorType.LogicalNegation:
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

        /// <summary>
        /// Determines whether the operator is left-to-right associative (true) or right-to-left (false).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool IsLeftAssociative(OperatorType type)
        {
            switch (type)
            {
                case OperatorType.PreDecrement:
                case OperatorType.PreIncrement:
                case OperatorType.UnaryMinus:
                case OperatorType.UnaryPlus:
                case OperatorType.LogicalNegation:
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

}
