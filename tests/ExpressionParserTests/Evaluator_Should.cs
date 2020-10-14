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
        private void GivenInput(string input, EvaluationContext context = null, bool coerse = false)
        {
            if (context == null) { context = new EvaluationContext(null); }

            _input = input;
            _result = new Evaluator(context).Evaluate(input, coerse).Value;
        }

        private void ExpectResult(string expected, string message = null)
        {
            Assert.AreEqual(expected, _result, message);
        }

        #endregion


        [TestMethod]
        public void Return_Null_When_Null()
        {
            GivenInput(null);
            ExpectResult(null);
        }

        [TestMethod]
        public void Return_Empty_When_Empty()
        {
            GivenInput("");
            ExpectResult("");
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
        public void Support_Currency_Formatted_Numbers_For_Simple_Calc()
        {
            GivenInput("$500 + $.30");
            ExpectResult("500.30");
        }

        [TestMethod]
        public void Support_Currency_Formatted_Field_For_Simple_Calc()
        {
            GivenInput("{whatever} + $.30", new EvaluationContext(field => ("$500", ValueTypeHint.Auto)));
            ExpectResult("500.30");
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
        public void Return_1_For_True_Literal_If_Coersing()
        {
            GivenInput("true", coerse: true);
            ExpectResult("1");

            GivenInput("1", coerse: true);
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_1_For_Non_False_Literal_If_Coersing()
        {
            GivenInput("whatev", coerse: true);
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_0_For_False_Literal_If_Coersing()
        {
            GivenInput("false", coerse: true);
            ExpectResult("0");

            GivenInput("0", coerse: true);
            ExpectResult("0");

            GivenInput("", coerse: true);
            ExpectResult("0");

            GivenInput(null, coerse: true);
            ExpectResult("0");
        }

        [TestMethod]
        public void Return_As_Is_If_Not_Coersing()
        {
            GivenInput("true", coerse: false);
            ExpectResult("true");

            GivenInput("1", coerse: false);
            ExpectResult("1");

            GivenInput("whatev", coerse: false);
            ExpectResult("whatev");

            GivenInput("false", coerse: false);
            ExpectResult("false");

            GivenInput("0", coerse: false);
            ExpectResult("0");

            GivenInput("", coerse: false);
            ExpectResult("");

            GivenInput(null, coerse: false);
            ExpectResult(null);
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
            GivenInput("{sample} == ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_0_For_NotEqualing_NullField_With_Empty_String()
        {
            GivenInput("{sample} != ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0");
        }

        [TestMethod]
        public void Support_Leading_0_Field_Value_As_Number_Comparison()
        {
            GivenInput("{field1} == {field2}", new EvaluationContext(field =>
            {
                switch (field)
                {
                    case "field1":
                        return ("01234", ValueTypeHint.Auto);
                    case "field2":
                        return ("001234", ValueTypeHint.Auto);
                }
                return ("", ValueTypeHint.Auto);
            }));
            ExpectResult("1");
        }
        [TestMethod]
        public void Support_Forcing_Field_As_String_Comparison()
        {
            GivenInput("{field1} == {field2}", new EvaluationContext(field =>
            {
                switch (field)
                {
                    case "field1":
                        return ("01234", ValueTypeHint.Text);
                    case "field2":
                        return ("001234", ValueTypeHint.Text);
                }
                return ("", ValueTypeHint.Text);
            }));
            ExpectResult("0");
        }

        // implicit boolean


        [TestMethod]
        public void Return_0_For_0_And_Y_Equality()
        {
            GivenInput("1 == 'Y'");
            ExpectResult("0");
        }

        [TestMethod]
        public void Return_1_For_Same_GLE()
        {
            GivenInput("1 >= 1");
            ExpectResult("1");

            GivenInput("1 <= 1");
            ExpectResult("1");
        }

        [TestMethod]
        public void Return_1_For_Simple_GLE()
        {
            GivenInput("5.1 >= 4.0");
            ExpectResult("1", "5.1 >= 4.0 fail");

            GivenInput("3.8 <= 4.0");
            ExpectResult("1", "3.8 <= 4.0 fail");
        }

        [TestMethod]
        public void One_And_Zero_Are_Implicit_True_And_False_For_Equals_Op()
        {
            GivenInput("1 == true", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "1st");
            GivenInput("true == 1", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "2nd");
            GivenInput("0 == true", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "3rd");
            GivenInput("true == 0", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "4th");


            GivenInput("1 == false", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "5th");
            GivenInput("false == 1", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "6th");
            GivenInput("0 == false", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "7th");
            GivenInput("false == 0", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "8th");
        }

        [TestMethod]
        public void One_And_Zero_Are_Implicit_True_And_False_For_NotEquals_Op()
        {
            GivenInput("1 != true", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "1st");
            GivenInput("true != 1", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "2nd");
            GivenInput("0 != true", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "3rd");
            GivenInput("true != 0", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "4th");


            GivenInput("1 != false", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "5th");
            GivenInput("false != 1", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "6th");
            GivenInput("0 != false", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "7th");
            GivenInput("false != 0", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "8th");
        }

        [TestMethod]
        public void Not_Implicitly_Use_Bool_Operation_Comparing_Empty_String_Against_Bool_String()
        {
            // all results should be false (0)

            GivenInput("'' == true", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "1st");
            GivenInput("true == ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "2nd");
            GivenInput("'' == 'true'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "3rd");
            GivenInput("'true' == ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "4th");


            GivenInput("'' == false", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "5th");
            GivenInput("false == ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "6th");
            GivenInput("'' == 'false'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "7th");
            GivenInput("'false' == ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "8th");
        }



        [TestMethod]
        public void Not_Implicitly_Use_Bool_Operation_Comparing_Non_Boolean_String_Against_Bool_String()
        {
            // all results should be false (0)

            GivenInput("'ABC' == true", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "1st");
            GivenInput("true == 'ABC'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "2nd");
            GivenInput("'ABC' == 'true'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "3rd");
            GivenInput("'true' == 'ABC'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "4th");


            GivenInput("'ABC' == false", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "5th");
            GivenInput("false == 'ABC'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "6th");
            GivenInput("'ABC' == 'false'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "7th");
            GivenInput("'false' == 'ABC'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "8th");
        }



        // date comparison

        [TestMethod]
        public void Handle_Two_Dates_Logical_Comparisons()
        {
            GivenInput("'2017/11/1' < '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "< failed");
            GivenInput("'2017/11/1' <= '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "<= failed");

            GivenInput("'2017/11/1' > '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "> failed");
            GivenInput("'2017/11/1' >= '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", ">= failed");

            GivenInput("'2017/11/1' == '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "== failed");
            GivenInput("'2017/11/1' == '2017/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "== failed 2");

            GivenInput("'2017/11/1' != '2017/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "!= failed");
            GivenInput("'2017/11/1' != '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "!= failed 2");
        }

        [TestMethod]
        public void Handle_Lhs_Date_Logical_Comparisons()
        {
            // empty date is assumed minimum

            GivenInput("'2017/11/1' < ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "< failed");
            GivenInput("'2017/11/1' <= ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "<= failed");

            GivenInput("'2017/11/1' > ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "> failed");
            GivenInput("'2017/11/1' >= ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", ">= failed");

            GivenInput("'2017/11/1' == ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "== failed");

            GivenInput("'2017/11/1' != ''", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "!= failed");
        }

        [TestMethod]
        public void Handle_Rhs_Date_Logical_Comparisons()
        {
            // empty date is assumed minimum

            GivenInput("'' < '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "< failed");
            GivenInput("'' <= '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "<= failed");

            GivenInput("'' > '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "> failed");
            GivenInput("'' >= '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", ">= failed");

            GivenInput("'' == '2018/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("0", "== failed");

            GivenInput("'' != '2017/11/1'", new EvaluationContext(field => (null, ValueTypeHint.Auto)));
            ExpectResult("1", "!= failed");
        }
    }
}
