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
        string _result;
        private void GivenInput(string input, EvaluationContext context = null)
        {
            if (context == null) { context = new EvaluationContext(null); }

            _input = input;
            _result = new Evaluator(context).Evaluate(input).Value;
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
        public void Handle_Unary_Minus()
        {
            GivenInput("100 + -50");
            ExpectResult("50");

            GivenInput("100 - -50");
            ExpectResult("150");
        }

        [TestMethod]
        public void Handle_Unary_Plus()
        {
            GivenInput("100 - +50");
            ExpectResult("50");

            GivenInput("100 + +50");
            ExpectResult("150");
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
        public void Handle_Unary_Negate()
        {
            GivenInput("!(100==100)");
            ExpectResult("0");

            GivenInput("!(100!=100)");
            ExpectResult("1");
        }

        [TestMethod]
        public void Handle_PreIncrement()
        {
            GivenInput("++100 + 5");
            ExpectResult("106");

            GivenInput("100 + ++5");
            ExpectResult("106");
        }

        [TestMethod]
        public void Handle_PreDecrement()
        {
            GivenInput("--100 + 5");
            ExpectResult("104");

            GivenInput("100 + --5");
            ExpectResult("104");
        }

        [TestMethod]
        public void Support_Basic_Function()
        {
            GivenInput("pow(2,8)");
            ExpectResult("256");
        }

        [TestMethod]
        public void Support_Custom_Function_With_BuiltIn_Operator()
        {
            var ctx = new EvaluationContext(null);
            ctx.RegisterFunction("always5", new FunctionRoutine(0, (c, p) => new ExpressionToken("5")));
            GivenInput("10 + always5()", ctx);
            ExpectResult("15");
        }

        [TestMethod]
        public void Support_Custom_Function_With_BuiltIn_Operator_Switch_Order()
        {
            var ctx = new EvaluationContext(null);
            ctx.RegisterFunction("always5", new FunctionRoutine(0, (c, p) => new ExpressionToken("5")));
            GivenInput("always5() + 10", ctx);
            ExpectResult("15");
        }

        [TestMethod]
        public void Return_1_For_True_Literal()
        {
            GivenInput("true");
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_0_For_Other_Literal()
        {
            GivenInput("whatev");
            ExpectResult("0");
        }

        [TestMethod]
        public void Return_1_For_True_Logical_And_Result()
        {
            GivenInput("true && true");
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_0_For_False_Logical_And_Result()
        {
            GivenInput("true && false");
            ExpectResult("0");
        }

        [TestMethod]
        public void Return_1_For_True_Logical_Or_Result()
        {
            GivenInput("false || true");
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_0_For_False_Logical_Or_Result()
        {
            GivenInput("false || asdf");
            ExpectResult("0");
        }



        [TestMethod]
        public void Return_0_For_Equaling_String_With_Empty_String()
        {
            GivenInput("howdy == ''");
            ExpectResult("0");
        }


        [TestMethod]
        public void Return_1_For_Equaling_String_With_Empty_String()
        {
            GivenInput("'' == ''");
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_0_For_NotEqualing_String_With_Empty_String()
        {
            GivenInput("'' != ''");
            ExpectResult("0");
        }

        [TestMethod]
        public void Return_1_For_Equaling_NullField_With_Empty_String()
        {
            GivenInput("{sample} == ''", new EvaluationContext(field => null));
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_0_For_NotEqualing_NullField_With_Empty_String()
        {
            GivenInput("{sample} != ''", new EvaluationContext(field=> null));
            ExpectResult("0");
        }
    }
}
