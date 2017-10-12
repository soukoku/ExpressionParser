using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Soukoku.ExpressionParser.Parsing
{
    [TestClass]
    public class InfixToPostfixTokenizer_Should
    {
        #region utility

        string _input;
        IList<ExpressionToken> _tokens;
        private void GivenInput(string input)
        {
            _input = input;
            _tokens = new InfixToPostfixTokenizer().Tokenize(input);
        }

        private void ExpectValues(params string[] expectedTokens)
        {
            var expectExpr = string.Join(" ", expectedTokens);
            var actualExpr = string.Join(" ", _tokens);

            CollectionAssert.AreEqual(expectedTokens, _tokens.Select(tk => tk.Value).ToList(),
                string.Format("Original \"{0}\", expected \"{1}\", got \"{2}\"", _input, expectExpr, actualExpr));
        }
        private void ExpectTypes(params ExpressionTokenType[] expectedTypes)
        {
            CollectionAssert.AreEqual(expectedTypes, _tokens.Select(tk => tk.TokenType).ToList());
        }

        #endregion


        [TestMethod]
        public void Return_Empty_List_When_Null()
        {
            GivenInput(null);
            ExpectValues();
        }
        [TestMethod]
        public void Return_Empty_List_When_Empty()
        {
            GivenInput("");
            ExpectValues();
        }

        [TestMethod]
        public void Return_Single_Value()
        {
            GivenInput("abc");
            ExpectValues("abc");
            ExpectTypes(ExpressionTokenType.Value);
        }

        [TestMethod]
        public void Ignore_White_Spaces()
        {
            GivenInput(" abc  123   ");
            ExpectValues("abc", "123");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Value);
        }

        [TestMethod]
        public void Reorder_Single_Operator()
        {
            GivenInput("ab + cd");
            ExpectValues("ab", "cd", "+");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Value, ExpressionTokenType.Operator);
        }

        [TestMethod]
        public void Reorder_Multiple_Operators_With_Same_Precedence()
        {
            GivenInput("ab + cd - 5");
            ExpectValues("ab", "cd", "+", "5", "-");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Value, ExpressionTokenType.Operator,
                ExpressionTokenType.Value, ExpressionTokenType.Operator);
        }

        [TestMethod]
        public void Reorder_Multiple_Operators_With_Different_Precedence()
        {
            GivenInput("ab + cd * 5");
            ExpectValues("ab", "cd", "5", "*", "+");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Value, ExpressionTokenType.Value,
                ExpressionTokenType.Operator, ExpressionTokenType.Operator);
        }


        // TODO: make better increment tests

        //[TestMethod]
        //public void Recognize_Ambiguous_PostIncrement_Operators()
        //{
        //    GivenInput("test+++ 5"); // the canonical ++ and + operators without space example
        //    ExpectValues("test", "5", "+", "++");
        //    ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator,
        //        ExpressionTokenType.Value, ExpressionTokenType.Operator);
        //}

        [TestMethod]
        public void Reorder_Parenthesis_Operation()
        {
            GivenInput("4 * (5 + cd)");
            ExpectValues("4", "5", "cd", "+", "*");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Value, ExpressionTokenType.Value,
                ExpressionTokenType.Operator, ExpressionTokenType.Operator);
        }

        [TestMethod]
        public void Read_Func_Without_Parameters()
        {
            GivenInput("Foo()");
            ExpectValues("Foo");
            ExpectTypes(ExpressionTokenType.Function);
        }
        [TestMethod]
        public void Read_Func_With_One_Parameter()
        {
            GivenInput("Foo(123)");
            ExpectValues("123", "Foo");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Function);
        }
        [TestMethod]
        public void Read_Func_With_Multiple_Parameters()
        {
            GivenInput("Foo(123, bar)");
            ExpectValues("123", "bar", "Foo");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Value, ExpressionTokenType.Function);
        }
        [TestMethod]
        public void Read_Func_Name_With_Dot()
        {
            GivenInput("Foo.Bar(xyz)");
            ExpectValues("xyz", "Foo.Bar");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Function);
        }

        [TestMethod]
        public void Read_Func_Without_Parameters_In_Expression()
        {
            GivenInput("Foo() == 1");
            ExpectValues("Foo", "1", "==");
            ExpectTypes(ExpressionTokenType.Function, ExpressionTokenType.Value, ExpressionTokenType.Operator);
        }
        [TestMethod]
        public void Read_Func_With_One_Parameter_In_Expression()
        {
            GivenInput("Foo(123) == 1");
            ExpectValues("123", "Foo", "1", "==");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Function, ExpressionTokenType.Value, ExpressionTokenType.Operator);
        }
    }
}
