using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soukoku.ExpressionParser
{
    /// <summary>
    /// Info on an operator.
    /// </summary>
    public class ExpressionOperator
    {
        // TODO: add all common code operators

        internal static readonly Dictionary<string, ExpressionOperator> KnownOperators = new Dictionary<string, ExpressionOperator>
        {
            // double char
            {"++", new ExpressionOperator{ Value= "++" } },
            {"--", new ExpressionOperator{ Value= "++" } },
            {"+=", new ExpressionOperator{ Value= "+=" } },
            {"-=", new ExpressionOperator{ Value= "-=" } },
            {"*=", new ExpressionOperator{ Value= "*=" } },
            {"/=", new ExpressionOperator{ Value= "/=" } },
            {"%=", new ExpressionOperator{ Value= "%=" } },
            {"==", new ExpressionOperator{ Value= "==" } },
            {"!=", new ExpressionOperator{ Value= "!=" } },
            {"<=", new ExpressionOperator{ Value= "<=" } },
            {">=", new ExpressionOperator{ Value= ">=" } },
            {"&&", new ExpressionOperator{ Value= "&&" } },
            {"||", new ExpressionOperator{ Value= "||" } },

            // single char
            {"+", new ExpressionOperator{ Value= "+" } },
            {"-", new ExpressionOperator{ Value= "-" } },
            {"*", new ExpressionOperator{ Value= "*" } },
            {"/", new ExpressionOperator{ Value= "/" } },
            {"=", new ExpressionOperator{ Value= "=" } },
            {"%", new ExpressionOperator{ Value= "%" } },
            {"^", new ExpressionOperator{ Value= "^" } },
            {"<", new ExpressionOperator{ Value= "<" } },
            {">", new ExpressionOperator{ Value= ">" } },
            {"~", new ExpressionOperator{ Value= "!" } },
            {"&", new ExpressionOperator{ Value= "&" } },
            {"|", new ExpressionOperator{ Value= "|" } },
            {"!", new ExpressionOperator{ Value= "!" } },
            //new ExpressionOperator{ Value= "[" },
            //new ExpressionOperator{ Value= "]" },
        };

        /// <summary>
        /// Gets the operator value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }
}
