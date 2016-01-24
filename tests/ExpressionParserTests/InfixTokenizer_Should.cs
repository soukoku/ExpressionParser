using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Soukoku.ExpressionParser
{
    [TestClass]
    public class InfixTokenizer_Should
    {
        #region utility

        string _input;
        IList<ExpressionToken> _tokens;
        private void GivenInput(string input)
        {
            _input = input;
            _tokens = new InfixTokenizer().Tokenize(input);
        }

        private void ExpectValues(params string[] expectedTokens)
        {
            CollectionAssert.AreEqual(expectedTokens, _tokens.Select(tk => tk.Value).ToList());
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
        public void Recognize_Single_Operator()
        {
            GivenInput("ab + cd");
            ExpectValues("ab", "+", "cd");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.Value);
        }


        [TestMethod]
        public void Recognize_Multiple_Operators()
        {
            GivenInput("ab + cd * 5");
            ExpectValues("ab", "+", "cd",
                "*", "5");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.Value,
                ExpressionTokenType.Operator, ExpressionTokenType.Value);
        }

        [TestMethod]
        public void Recognize_MultiChar_Operators()
        {
            GivenInput("test && ab++ * 5");
            ExpectValues("test", "&&", "ab",
                "++", "*", "5");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.Value,
                ExpressionTokenType.Operator, ExpressionTokenType.Operator, ExpressionTokenType.Value);
        }

        [TestMethod]
        public void Recognize_Ambiguous_MultiChar_Operators()
        {
            GivenInput("test+++ 5"); // the canonical ++ and + operators without space example
            ExpectValues("test", "++", "+", "5");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator,
                ExpressionTokenType.Operator, ExpressionTokenType.Value);
        }


        [TestMethod]
        public void Recognize_Parenthesis()
        {
            GivenInput("4 * (5 + cd)");
            ExpectValues("4", "*", "(", "5", "+", "cd", ")");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.OpenParenthesis,
                ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.Value,
                ExpressionTokenType.CloseParenthesis);
        }

        [TestMethod]
        public void Read_Between_Braces_As_Field()
        {
            GivenInput(" {asdf qwerty} ");
            ExpectValues("asdf qwerty");
            ExpectTypes(ExpressionTokenType.Field);
        }

        [TestMethod]
        public void Read_Between_Double_Quotes()
        {
            GivenInput(" \"you know it\" ");
            ExpectValues("you know it");
            ExpectTypes(ExpressionTokenType.DoubleQuoted);
        }

        [TestMethod]
        public void Read_Between_Single_Quotes()
        {
            GivenInput(" 'does it work?' ");
            ExpectValues("does it work?");
            ExpectTypes(ExpressionTokenType.SingleQuoted);
        }

        // TODO: test escaped quotes

        [TestMethod]
        public void Read_Func_Without_Parameters()
        {
            GivenInput("Foo()");
            ExpectValues("Foo", "(", ")");
            ExpectTypes(ExpressionTokenType.Function, ExpressionTokenType.OpenParenthesis, ExpressionTokenType.CloseParenthesis);
        }
        [TestMethod]
        public void Read_Func_With_One_Parameter()
        {
            GivenInput("Foo(123)");
            ExpectValues("Foo", "(", "123", ")");
            ExpectTypes(ExpressionTokenType.Function, ExpressionTokenType.OpenParenthesis, ExpressionTokenType.Value, ExpressionTokenType.CloseParenthesis);
        }
        [TestMethod]
        public void Read_Func_With_Multiple_Parameters()
        {
            GivenInput("Foo(123, bar)");
            ExpectValues("Foo", "(", "123", ",", "bar", ")");
            ExpectTypes(ExpressionTokenType.Function, ExpressionTokenType.OpenParenthesis, ExpressionTokenType.Value,
                ExpressionTokenType.Comma, ExpressionTokenType.Value, ExpressionTokenType.CloseParenthesis);
        }
        [TestMethod]
        public void Read_Func_Name_With_Dot()
        {
            GivenInput("Foo.Bar(xyz)");
            ExpectValues("Foo.Bar", "(", "xyz", ")");
            ExpectTypes(ExpressionTokenType.Function, ExpressionTokenType.OpenParenthesis, ExpressionTokenType.Value, ExpressionTokenType.CloseParenthesis);
        }

    }
}
