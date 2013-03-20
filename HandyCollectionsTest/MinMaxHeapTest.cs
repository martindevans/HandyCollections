using System;
using System.Collections.Generic;
using System.Linq;
using HandyCollections;
using HandyCollections.Extensions;
using HandyCollections.Heap;
using HandyCollections.RandomNumber;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class MinMaxHeapTest
    {
        private MinMaxHeap_Accessor<int> _intHeap;

        [TestInitialize]
        public void TestSetup()
        {
            _intHeap = new MinMaxHeap_Accessor<int>();
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
        public void IsMinLevel()
        {
            int levelLength = 1;
            bool minlevel = true;
            int nextSwitch = 1;
            for (int i = 0; i < 30000; i++)
            {
                if (i == nextSwitch)
                {
                    minlevel = !minlevel;
                    levelLength *= 2;
                    nextSwitch += levelLength;
                }

                if (minlevel)
                    Assert.IsTrue(MinMaxHeap_Accessor<int>.IsMinLevel(i));
                else
                    Assert.IsFalse(MinMaxHeap_Accessor<int>.IsMinLevel(i));
            }
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
            }

            Assert.AreEqual(numbers.Count, _intHeap.Count);
        }

        [TestMethod]
        public void IndexOfHeapItemIsCorrect()
        {
            List<int> numbers = new List<int>();
            Random r = new Random(235246);
            for (int i = 0; i < 1000; i++)
                numbers.Add(r.Next(0, 1000));

            _intHeap.AddMany(numbers);

            for (int i = 0; i < _intHeap.Count; i++)
            {
                var index = _intHeap.IndexOf(_intHeap._heap[i]);
                Assert.IsTrue(i >= index);
            }
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

        [TestMethod]
        public void MinMaxMedianHeap()
        {
            MinMaxMedianHeap<int> testHeap = new MinMaxMedianHeap<int>();

            testHeap.Add(1);
            CheckValues(testHeap, 1, 1, 1);

            testHeap.Add(2);
            CheckValues(testHeap, 1, 1, 2);

            testHeap.Add(3);
            CheckValues(testHeap, 1, 2, 3);

            testHeap.RemoveMax();
            CheckValues(testHeap, 1, 1, 2);

            testHeap.Add(3);
            CheckValues(testHeap, 1, 2, 3);

            testHeap.RemoveMedian();
            CheckValues(testHeap, 1, 1, 3);

            testHeap.Add(2);
            CheckValues(testHeap, 1, 2, 3);

            testHeap.RemoveMin();
            CheckValues(testHeap, 2, 2, 3);

            testHeap.Add(1);
            CheckValues(testHeap, 1, 2, 3);

            testHeap.Add(4);
            CheckValues(testHeap, 1, 2, 4);

            testHeap.Add(5);
            CheckValues(testHeap, 1, 3, 5);

            testHeap.Add(6);
            testHeap.Add(7);
            CheckValues(testHeap, 1, 4, 7);

            testHeap.Add(6);
            testHeap.Add(7);
            CheckValues(testHeap, 1, 5, 7);

            for (int i = 0; i < 100; i++)
            {
                int[] values = new int[100];
                for (int v = 0; v < values.Length; v++)
                {
                    values[v] = (int)StaticRandomNumber.Random(0, int.MaxValue);
                }

                int min = values.Min();
                int max = values.Max();
                int median = values[values.OrderSelect(values.Length / 2)];

                testHeap.Clear();
                testHeap.AddMany(values);
                CheckValues(testHeap, min, median, max);
            }
        }

        private static void CheckValues(MinMaxMedianHeap<int> testHeap, int expectedMin, int expectedMedian, int expectedMax)
        {
            Assert.AreEqual(testHeap.Maximum, expectedMax);
            Assert.AreEqual(testHeap.Minimum, expectedMin);
            Assert.AreEqual(testHeap.Median, expectedMedian);
        }
    }
}
