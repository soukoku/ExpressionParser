using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Soukoku.ExpressionParser.Parsing
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
            GivenInput("test && ab <= 5");
            ExpectValues("test", "&&", "ab", "<=", "5");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.Value,
                ExpressionTokenType.Operator, ExpressionTokenType.Value);
        }

        [TestMethod]
        public void Recognize_PreIncrement_Operators()
        {
            GivenInput("++test + ++5");
            ExpectValues("++", "test", "+", "++", "5");
            ExpectTypes(ExpressionTokenType.Operator, ExpressionTokenType.Value, ExpressionTokenType.Operator, 
                ExpressionTokenType.Operator, ExpressionTokenType.Value);

            Assert.AreEqual(OperatorType.PreIncrement, _tokens[0].OperatorType);
            Assert.AreEqual(OperatorType.Addition, _tokens[2].OperatorType);
            Assert.AreEqual(OperatorType.PreIncrement, _tokens[3].OperatorType);
        }

        [TestMethod]
        public void Recognize_PreDecrement_Operators()
        {
            GivenInput("--test - --5");
            ExpectValues("--", "test", "-", "--", "5");
            ExpectTypes(ExpressionTokenType.Operator, ExpressionTokenType.Value, ExpressionTokenType.Operator,
                ExpressionTokenType.Operator, ExpressionTokenType.Value);

            Assert.AreEqual(OperatorType.PreDecrement, _tokens[0].OperatorType);
            Assert.AreEqual(OperatorType.Subtraction, _tokens[2].OperatorType);
            Assert.AreEqual(OperatorType.PreDecrement, _tokens[3].OperatorType);
        }

        [TestMethod]
        public void Recognize_PostIncrement_Operators()
        {
            GivenInput("test++ + 5");
            ExpectValues("test", "++", "+", "5");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator,
                ExpressionTokenType.Operator, ExpressionTokenType.Value);

            Assert.AreEqual(OperatorType.PostIncrement, _tokens[1].OperatorType);
            Assert.AreEqual(OperatorType.Addition, _tokens[2].OperatorType);
        }

        [TestMethod]
        public void Recognize_PostDecrement_Operators()
        {
            GivenInput("test-- - 5");
            ExpectValues("test", "--", "-", "5");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator,
                ExpressionTokenType.Operator, ExpressionTokenType.Value);

            Assert.AreEqual(OperatorType.PostDecrement, _tokens[1].OperatorType);
            Assert.AreEqual(OperatorType.Subtraction, _tokens[2].OperatorType);
        }

        [TestMethod]
        public void Recognize_Ambiguous_PostIncrement_Operators()
        {
            GivenInput("test+++ 5"); // the canonical ++ and + operators without space example
            ExpectValues("test", "++", "+", "5");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator,
                ExpressionTokenType.Operator, ExpressionTokenType.Value);

            Assert.AreEqual(OperatorType.PostIncrement, _tokens[1].OperatorType);
            Assert.AreEqual(OperatorType.Addition, _tokens[2].OperatorType);
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

        // TODO: test more escaped quotes

        [TestMethod]
        public void Read_Escaped_Single_Quotes()
        {
            GivenInput("'A\\'s letter' ");
            ExpectValues("A's letter");
            ExpectTypes(ExpressionTokenType.SingleQuoted);
        }

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

        [TestMethod]
        public void Handle_Unary_PlusMinus_As_First_Token()
        {
            GivenInput("-3");
            ExpectValues("-", "3");
            ExpectTypes(ExpressionTokenType.Operator, ExpressionTokenType.Value);
            Assert.AreEqual(OperatorType.UnaryMinus, _tokens[0].OperatorType);

            GivenInput("+3");
            ExpectValues("+", "3");
            ExpectTypes(ExpressionTokenType.Operator, ExpressionTokenType.Value);
            Assert.AreEqual(OperatorType.UnaryPlus, _tokens[0].OperatorType);
        }

        [TestMethod]
        public void Handle_Unary_PlusMinus_After_Operator()
        {
            GivenInput("5 + -3");
            ExpectValues("5", "+", "-", "3");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.Operator, ExpressionTokenType.Value);
            Assert.AreEqual(OperatorType.Addition, _tokens[1].OperatorType);
            Assert.AreEqual(OperatorType.UnaryMinus, _tokens[2].OperatorType);

            GivenInput("5 - +3");
            ExpectValues("5", "-", "+", "3");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.Operator, ExpressionTokenType.Value);
            Assert.AreEqual(OperatorType.Subtraction, _tokens[1].OperatorType);
            Assert.AreEqual(OperatorType.UnaryPlus, _tokens[2].OperatorType);
        }
    }
}
