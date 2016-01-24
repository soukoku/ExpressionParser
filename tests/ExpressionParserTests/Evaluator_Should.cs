﻿using System;
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
        string _result;
        private void GivenInput(string input, EvaluationContext context = null)
        {
            if (context == null) { context = new EvaluationContext(null); }

            _input = input;
            _result = new Evaluator(context).EvaluateInfix(input).Value;
        }

        private void ExpectResult(string expected)
        {
            Assert.AreEqual(expected, _result);
        }

        #endregion


        [TestMethod]
        public void Return_0_When_Null()
        {
            GivenInput(null);
            ExpectResult("0");
        }

        [TestMethod]
        public void Return_0_When_Empty()
        {
            GivenInput("");
            ExpectResult("0");
        }

        [TestMethod]
        public void Return_Single_Value()
        {
            GivenInput("100");
            ExpectResult("100");
        }

        [TestMethod]
        public void Handle_Simple_Operation()
        {
            GivenInput("100 + 100");
            ExpectResult("200");
        }

        [TestMethod]
        public void Handle_Multi_Operation_With_Same_Precedence()
        {
            GivenInput("100 + 50 - 50");
            ExpectResult("100");
        }

        [TestMethod]
        public void Handle_Multi_Operation_With_Different_Precedence()
        {
            GivenInput("100 + 2 * 10");
            ExpectResult("120");
        }

        [TestMethod]
        public void Handle_Multi_Operation_With_Parenthesis()
        {
            GivenInput("(100 + 2) * 10");
            ExpectResult("1020");

            GivenInput("100 * (2 + 10)");
            ExpectResult("1200");
        }


        [TestMethod]
        public void Use_1_OR_0_For_Logical_Result()
        {
            GivenInput("100 == 100");
            ExpectResult("1");

            GivenInput("100 != 100");
            ExpectResult("0");
        }

        [TestMethod]
        public void Support_Basic_Function()
        {
            GivenInput("pow(2,8)");
            ExpectResult("256");
        }

        [TestMethod]
        public void Support_Custom_Function()
        {
            var ctx = new EvaluationContext(null);
            ctx.RegisterFunction("always5", new FunctionInfo(0, (c, p) => new ExpressionToken("5")));
            GivenInput("always5()", ctx);
            ExpectResult("5");
        }
    }
}
