using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AssertThat_IndexingAfterCount_Throws()
        {
            var r = new RingBuffer<int>(5)
            {
                1,
                2
            };


            var v = r[3];
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
    }
}
