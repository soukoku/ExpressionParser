using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Soukoku.ExpressionParser
{
    [TestClass]
    public class RawTokenizer_Should
    {
        #region utility

        private void ExpectTokens(params string[] expectedTokens)
        {
            var actualTokens = new RawTokenizer().Tokenize(_input);
            CollectionAssert.AreEqual(expectedTokens, actualTokens.Select(tk => tk.Value).ToList());
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
            ExpectTokens();
        }
        [TestMethod]
        public void Return_Empty_List_When_Empty()
        {
            GivenInput("");
            ExpectTokens();
        }

        [TestMethod]
        public void Return_Single_Char_Value()
        {
            GivenInput("a");
            ExpectTokens("a");
        }

        [TestMethod]
        public void Return_Single_MultiChar_Value()
        {
            GivenInput("ab");
            ExpectTokens("ab");
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Whitespace()
        {
            GivenInput("ab cd");
            ExpectTokens("ab", " ", "cd");
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Symbols()
        {
            GivenInput("ab+cd");
            ExpectTokens("ab", "+", "cd");
        }

        [TestMethod]
        public void Return_Multiple_Values_Split_By_Symbols_And_Whitespace()
        {
            GivenInput(" ab  +  cd ");
            ExpectTokens(" ", "ab", "  ", "+", "  ", "cd", " ");
        }
    }
}
