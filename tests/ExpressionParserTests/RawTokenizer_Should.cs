using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Soukoku.ExpressionParser
{
    [TestClass]
    public class RawTokenizer_Should
    {
        #region utility

        private void ExpectValues(params string[] expectedTokens)
        {
            var actualTokens = new RawTokenizer().Tokenize(_input);

            // check each tokens
            CollectionAssert.AreEqual(expectedTokens, actualTokens.Select(tk => tk.Value).ToList());

            // check reconstructed input
            if (!string.IsNullOrEmpty(_input))
                Assert.AreEqual(_input, string.Join("", actualTokens));
        }

        string _input;
        private void GivenInput(string input)
        {
            _input = input;
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
        }

        [TestMethod]
        public void Return_Single_MultiChar_Value()
        {
            GivenInput("ab");
            ExpectValues("ab");
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Whitespace()
        {
            GivenInput("ab cd");
            ExpectValues("ab", " ", "cd");
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Symbols()
        {
            GivenInput("ab+cd");
            ExpectValues("ab", "+", "cd");
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Symbols_And_Whitespace()
        {
            GivenInput(" ab  +  cd ");
            ExpectValues(" ", "ab", "  ", "+", "  ", "cd", " ");
        }

        [TestMethod]
        public void Return_Operators_In_Singles()
        {
            // the == and ++ should both be parsed as 2 operators in this phase

            GivenInput("13 == ab++  +  cd ");
            ExpectValues("13", " ", "=", "=", " ", "ab", "+", "+", "  ", "+", "  ", "cd", " ");
        }
    }
}
