using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace Soukoku.ExpressionParser.Parsing
{
    [TestClass]
    public class RawTokenizer_Should
    {
        #region utility

        string _input;
        IList<RawToken> _tokens;
        private void GivenInput(string input)
        {
            _input = input;
            _tokens = new RawTokenizer().Tokenize(input);
        }

        private void ExpectValues(params string[] expectedTokens)
        {
            // check each tokens
            CollectionAssert.AreEqual(expectedTokens, _tokens.Select(tk => tk.Value).ToList());

            // check reconstructed input
            if (!string.IsNullOrEmpty(_input))
                Assert.AreEqual(_input, string.Join("", _tokens));
        }
        private void ExpectTypes(params RawTokenType[] expectedTypes)
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
        public void Return_Single_Char_Value()
        {
            GivenInput("a");
            ExpectValues("a");
            ExpectTypes(RawTokenType.Literal);
        }

        [TestMethod]
        public void Return_Single_MultiChar_Value()
        {
            GivenInput("ab");
            ExpectValues("ab");
            ExpectTypes(RawTokenType.Literal);
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Whitespace()
        {
            GivenInput("ab cd");
            ExpectValues("ab", " ", "cd");
            ExpectTypes(RawTokenType.Literal, RawTokenType.WhiteSpace, RawTokenType.Literal);
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Symbols()
        {
            GivenInput("ab+cd");
            ExpectValues("ab", "+", "cd");
            ExpectTypes(RawTokenType.Literal, RawTokenType.Symbol, RawTokenType.Literal);
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Symbols_And_Whitespace()
        {
            GivenInput(" ab  +  cd ");
            ExpectValues(" ", "ab", 
                "  ", "+", 
                "  ", "cd", 
                " ");
            ExpectTypes(RawTokenType.WhiteSpace, RawTokenType.Literal, 
                RawTokenType.WhiteSpace, RawTokenType.Symbol, 
                RawTokenType.WhiteSpace, RawTokenType.Literal,
                RawTokenType.WhiteSpace);
        }

        [TestMethod]
        public void Return_Operators_In_Singles()
        {
            // the == and ++ should both be parsed as 2 operators in this phase

            GivenInput("13 == ab++  +  cd");
            ExpectValues("13", " ", 
                "=", "=", 
                " ", "ab", 
                "+", "+", 
                "  ", "+", 
                "  ", "cd");
            ExpectTypes(RawTokenType.Literal, RawTokenType.WhiteSpace,
                RawTokenType.Symbol, RawTokenType.Symbol,
                RawTokenType.WhiteSpace, RawTokenType.Literal,
                RawTokenType.Symbol, RawTokenType.Symbol,
                RawTokenType.WhiteSpace, RawTokenType.Symbol,
                RawTokenType.WhiteSpace, RawTokenType.Literal);
        }
    }
}
