using System;
using System.Collections.Generic;
using HandyCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class RingBufferTest
    {
        [TestMethod]
        public void AssertThat_ConstructingRingBuffer_DoesNotThrow()
        {
            var r = new RingBuffer<int>(5);

            Assert.AreEqual(0, r.Count);
            Assert.AreEqual(5, r.Capacity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AssertThat_ConstructingWithNegativeCapacity_Throws()
        {
            // ReSharper disable once UnusedVariable
            var r = new RingBuffer<int>(-5);
        }

        [TestMethod]
        public void AssertThat_AddingToRingBuffer_AddsItems()
        {
            var r = new RingBuffer<int>(5)
            {
                1,
                2
            };


            Assert.AreEqual(1, r[0]);
            Assert.AreEqual(2, r[1]);
        }

        [TestMethod]
        public void AssertThat_AddingArrayToRingBuffer_AddsItems()
        {
            var r = new RingBuffer<int>(5) { new int[] {1, 2} };


            Assert.AreEqual(1, r[0]);
            Assert.AreEqual(2, r[1]);
        }

        [TestMethod]
        public void AssertThat_AddingArrayToRingBuffer_Overwrites()
        {
            var r = new RingBuffer<int>(5) {

                //Add so much data we completely overwrite the entire array
                new int[] {
                    1, 2, 3, 4, 5,
                    6, 7, 8, 9, 10,
                    11
                }
            };

            Assert.AreEqual(7, r[0]);
            Assert.AreEqual(8, r[1]);
        }

        [TestMethod]
        public void AssertThat_AddingArrayToRingBuffer_Wraps()
        {
            var r = new RingBuffer<int>(5) {

                //Add some initial data
                1,
                2,
                3,

                //Add some data which will fall off the end
                new int[] {4, 5, 6, 7}
            };

            Assert.AreEqual(3, r[0]);
            Assert.AreEqual(4, r[1]);
            Assert.AreEqual(5, r[2]);
            Assert.AreEqual(6, r[3]);
            Assert.AreEqual(7, r[4]);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AssertThat_IndexingAfterCount_Throws()
        {
            var r = new RingBuffer<int>(5)
            {
                1,
                2
            };


            // ReSharper disable once UnusedVariable
            var v = r[3];
        }

        [TestMethod]
        public void AssertThat_CopyTo_CopiesCompleteBuffer_WhenBufferIsNotFull()
        {
            var r = new RingBuffer<int>(5) { 1, 2, 3 };

            var output = r.CopyTo(new ArraySegment<int>(new int[10]));

            Assert.IsNotNull(output.Array);

            Assert.AreEqual(3, output.Count);

            Assert.AreEqual(1, output.Array[0]);
            Assert.AreEqual(2, output.Array[1]);
            Assert.AreEqual(3, output.Array[2]);
        }

        [TestMethod]
        public void AssertThat_CopyTo_CopiesCompleteBuffer_WhenBufferIsTorn()
        {
            //Write enough data to wrap around so the buffer is "torn"
            var r = new RingBuffer<int>(5) { 1, 2, 3, 4, 5, 6 };

            var output = r.CopyTo(new ArraySegment<int>(new int[10]));

            Assert.IsNotNull(output.Array);

            Assert.AreEqual(5, output.Count);

            Assert.AreEqual(2, output.Array[0]);
            Assert.AreEqual(3, output.Array[1]);
            Assert.AreEqual(4, output.Array[2]);
            Assert.AreEqual(5, output.Array[3]);
            Assert.AreEqual(6, output.Array[4]);
        }

        [TestMethod]
        public void AssertThat_AddingMoreThanCapacity_RemovesOldestValue()
        {
            var r = new RingBuffer<int>(5);
            var l = new List<int>();

            //We'll use the tail of a list to emulate a ringbuffer and check that the actual ringbuffer always contains the same values

            for (var i = 0; i < 1000; i++)
            {
                //Add to list - last 5 items in list should be items in ringbuffer
                l.Add(i);

                //Add to ring buffer
                r.Add(i);

                //Ensure two buffers are exactly the same
                for (int j = 0; j < r.Count; j++)
                {
                    Assert.AreEqual(l[l.Count - r.Count + j], r[j]);
                }
            }
        }

        [TestMethod]
        public void AssertThat_Clear_ClearsAllData()
        {
            var b = new RingBuffer<int>(4) { 1, 2, 3, 4, 5 };

            Assert.AreEqual(4, b.Count);
            b.Clear();
            Assert.AreEqual(0, b.Count);
        }
    }
}
