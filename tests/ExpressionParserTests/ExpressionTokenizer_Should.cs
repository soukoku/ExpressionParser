using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Soukoku.ExpressionParser
{
    [TestClass]
    public class ExpressionTokenizer_Should
    {
        #region utility

        private void ExpectValues(params string[] expectedTokens)
        {
            CollectionAssert.AreEqual(expectedTokens, _tokens.Select(tk => tk.Value).ToList());
        }
        private void ExpectTypes(params ExpressionTokenType[] expectedTypes)
        {
            CollectionAssert.AreEqual(expectedTypes, _tokens.Select(tk => tk.TokenType).ToList());
        }

        string _input;
        IList<ExpressionToken> _tokens;
        private void GivenInput(string input)
        {
            _input = input;
            _tokens = new ExpressionTokenizer().Tokenize(_input);
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
        public void Recognizes_Single_Operator()
        {
            GivenInput("ab + cd");
            ExpectValues("ab", "+", "cd");
            ExpectTypes(ExpressionTokenType.Value, ExpressionTokenType.Operator, ExpressionTokenType.Value);
        }


    }
}
