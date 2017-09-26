using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace Soukoku.ExpressionParser.Parsing
{
    [TestClass]
    public class ListReader_Should
    {
        #region utility

        private ListReader<int> GivenList(params int[] values)
        {
            return new ListReader<int>(values);
        }

        #endregion


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_ArgNull_On_Null_List()
        {
            GivenList(null);
        }

        [TestMethod]
        public void Start_Position_At_0()
        {
            var reader = GivenList(new int[0]);
            Assert.AreEqual(0, reader.Position);
            Assert.IsTrue(reader.IsEnd);
        }

        [TestMethod]
        public void Move_Position_Forward_On_ReadNext()
        {
            var reader = GivenList(5);
            Assert.AreEqual(5, reader.Read());
            Assert.AreEqual(1, reader.Position);
        }

        [TestMethod]
        public void Keep_Position_On_Peek()
        {
            var reader = GivenList(5);
            Assert.AreEqual(5, reader.Peek());
            Assert.AreEqual(0, reader.Position);
        }

        [TestMethod]
        public void Keep_Position_On_Peek_Offset()
        {
            var reader = GivenList(5);
            reader.Read();
            Assert.AreEqual(5, reader.Peek(-1));
            Assert.AreEqual(1, reader.Position);
        }

        [TestMethod]
        public void Make_Position_Same_As_List_Length_At_End()
        {
            var reader = GivenList(5, 6, 7);
            while (!reader.IsEnd)
            {
                reader.Read();
            }
            Assert.AreEqual(3, reader.Position);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Throw_ArgOutRange_When_Read_Beyond_List()
        {
            var reader = GivenList(1);
            reader.Read();
            reader.Read();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Throw_ArgOutRange_When_Peek_Beyond_List()
        {
            var reader = GivenList(1);
            reader.Read();
            reader.Peek();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Throw_ArgOutRange_When_Peek_Offset_Beyond_List()
        {
            var reader = GivenList(1);
            reader.Peek(-1);
        }

        [TestMethod]
        public void Read_From_Set_Position()
        {
            var reader = GivenList(5, 6, 7);
            reader.Position = 1;
            Assert.AreEqual(1, reader.Position);
            Assert.AreEqual(6, reader.Read());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Throw_ArgOutRange_When_Position_Is_Bad()
        {
            var reader = GivenList(5, 6, 7);
            reader.Position = 4;
        }
    }
}
