using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandyCollections.Heap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class MinMaxMedianHeapTest
    {
        private MinMaxMedianHeap_Accessor<int> _intHeap;

        [TestInitialize]
        public void TestSetup()
        {
            _intHeap = new MinMaxMedianHeap_Accessor<int>();
        }

        private List<int> PopulateHeap(int seed, int count)
        {
            List<int> values = new List<int>();

            Random r = new Random(seed);
            for (int i = 0; i < count; i++)
            {
                var v = r.Next();
                _intHeap.Add(v);
                values.Add(v);
            }

            return values;
        }

        [TestMethod]
        public void BulkRandomNumbersBuildValidHeap()
        {
            List<int> numbers = new List<int>();
            Random r = new Random(235246);
            for (int i = 0; i < 1000; i++)
                numbers.Add(r.Next(0, 1000));

            _intHeap.AddMany(numbers);

            Assert.AreEqual(numbers.Max(), _intHeap.Max());
            Assert.AreEqual(numbers.Min(), _intHeap.Min());

            numbers.Sort();
            Assert.AreEqual(numbers[numbers.Count / 2], _intHeap.HighMedian);
        }

        [TestMethod]
        public void ManySingleRandomNumbersBuildValidHeap()
        {
            Random r = new Random(12423);

            List<int> numbers = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                var v = r.Next();
                numbers.Add(v);
                _intHeap.Add(v);

                Assert.AreEqual(numbers.Max(), _intHeap.Max());
                Assert.AreEqual(numbers.Min(), _intHeap.Min());

                numbers.Sort();
                Assert.AreEqual(numbers[numbers.Count / 2], _intHeap.HighMedian);
            }

            Assert.AreEqual(numbers.Count, _intHeap.Count);
        }


        [TestMethod]
        public void RemoveMaxSuccessfullyUpdatesMinAndMax()
        {
            var numbers = PopulateHeap(23521, 1000);
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(numbers.Max(), _intHeap.Maximum);
                Assert.AreEqual(numbers.Min(), _intHeap.Minimum);

                numbers.Remove(_intHeap.Maximum);
                _intHeap.RemoveMax();
            }
        }

        [TestMethod]
        public void RemoveMinSuccessfullyUpdatesMinAndMax()
        {
            var numbers = PopulateHeap(3742, 1000);
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(numbers.Max(), _intHeap.Maximum);
                Assert.AreEqual(numbers.Min(), _intHeap.Minimum);

                numbers.Remove(_intHeap.Minimum);
                _intHeap.RemoveMin();
            }
        }
    }
}
