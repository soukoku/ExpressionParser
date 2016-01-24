using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace Soukoku.ExpressionParser
{
    [TestClass]
    public class Evaluator_Should
    {
        #region utility

        string _input;
        decimal _result;
        private void GivenInput(string input)
        {
            _input = input;
            _result = new Evaluator(new EvaluationContext()).EvaluateInfix(input);
        }

        private void ExpectResult(decimal expected)
        {
            Assert.AreEqual(expected, _result);
        }

        #endregion


        [TestMethod]
        public void Return_0_When_Null()
        {
            GivenInput(null);
            ExpectResult(0);
        }

        [TestMethod]
        public void Return_0_When_Empty()
        {
            GivenInput("");
            ExpectResult(0);
        }

        [TestMethod]
        public void Return_Single_Value()
        {
            GivenInput("100");
            ExpectResult(100);
        }

        [TestMethod]
        public void Handle_Simple_Operation()
        {
            GivenInput("100 + 100");
            ExpectResult(200);
        }

    }
}
