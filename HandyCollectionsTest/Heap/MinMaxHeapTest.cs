//using System;
//using System.Collections.Generic;
//using System.Linq;
//using HandyCollections;
//using HandyCollections.Extensions;
//using HandyCollections.Heap;
//using HandyCollections.RandomNumber;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace HandyCollectionsTest
//{
//    [TestClass]
//    public class MinMaxHeapTest
//    {
//        private MinMaxHeap_Accessor<int> _intHeap;

//        [TestInitialize]
//        public void TestSetup()
//        {
//            _intHeap = new MinMaxHeap_Accessor<int>();
//        }

//        private List<int> PopulateHeap(int seed, int count)
//        {
//            List<int> values = new List<int>();

//            Random r = new Random(seed);
//            for (int i = 0; i < count; i++)
//            {
//                var v = r.Next();
//                _intHeap.Add(v);
//                values.Add(v);
//            }

//            AssertHeap(_intHeap);

//            return values;
//        }

//        [TestMethod]
//        public void IsMinLevel()
//        {
//            int levelLength = 1;
//            bool minlevel = true;
//            int nextSwitch = 1;
//            for (int i = 0; i < 30000; i++)
//            {
//                if (i == nextSwitch)
//                {
//                    minlevel = !minlevel;
//                    levelLength *= 2;
//                    nextSwitch += levelLength;
//                }

//                if (minlevel)
//                    Assert.IsTrue(MinMaxHeap_Accessor<int>.IsMinLevel(i));
//                else
//                    Assert.IsFalse(MinMaxHeap_Accessor<int>.IsMinLevel(i));
//            }
//        }

//        [TestMethod]
//        public void BulkRandomNumbersBuildValidHeap()
//        {
//            List<int> numbers = new List<int>();
//            Random r = new Random(235246);
//            for (int i = 0; i < 1000; i++)
//                numbers.Add(r.Next(0, 1000));

//            _intHeap.AddMany(numbers);

//            AssertHeap(_intHeap);
//            Assert.AreEqual(numbers.Max(), _intHeap.Max());
//            Assert.AreEqual(numbers.Min(), _intHeap.Min());
//        }

//        [TestMethod]
//        public void ManySingleRandomNumbersBuildValidHeap()
//        {
//            Random r = new Random(12423);

//            List<int> numbers = new List<int>();
//            for (int i = 0; i < 10000; i++)
//            {
//                var v = r.Next();
//                numbers.Add(v);
//                _intHeap.Add(v);

//                AssertHeap(_intHeap);
//                Assert.AreEqual(numbers.Max(), _intHeap.Max());
//                Assert.AreEqual(numbers.Min(), _intHeap.Min());
//            }

//            Assert.AreEqual(numbers.Count, _intHeap.Count);
//        }

//        [TestMethod]
//        public void IndexOfHeapItemIsCorrect()
//        {
//            List<int> numbers = new List<int>();
//            Random r = new Random(235246);
//            for (int i = 0; i < 1000; i++)
//                numbers.Add(r.Next(0, 1000));

//            _intHeap.AddMany(numbers);

//            for (int i = 0; i < _intHeap.Count; i++)
//            {
//                var index = _intHeap.IndexOf(_intHeap._heap[i]);
//                Assert.IsTrue(i >= index);
//            }
//        }

//        [TestMethod]
//        public void RemoveMaxSuccessfullyUpdatesMinAndMax()
//        {
//            var numbers = PopulateHeap(23521, 1000);
//            for (int i = 0; i < 100; i++)
//            {
//                Assert.AreEqual(numbers.Max(), _intHeap.Maximum);
//                Assert.AreEqual(numbers.Min(), _intHeap.Minimum);

//                numbers.Remove(_intHeap.Maximum);
//                _intHeap.RemoveMax();
//            }
//        }

//        [TestMethod]
//        public void RemoveMinSuccessfullyUpdatesMinAndMax()
//        {
//            var numbers = PopulateHeap(3742, 1000);
//            for (int i = 0; i < 100; i++)
//            {
//                Assert.AreEqual(numbers.Max(), _intHeap.Maximum);
//                Assert.AreEqual(numbers.Min(), _intHeap.Minimum);

//                numbers.Remove(_intHeap.Minimum);
//                _intHeap.RemoveMin();
//            }
//        }

//        [TestMethod]
//        public void MineMaxHeapUpdateKeySmaller()
//        {
//            MinMaxHeap_Accessor<HeapItem> heap = new MinMaxHeap_Accessor<HeapItem>();

//            List<HeapItem> items = new List<HeapItem>();
//            for (int i = 0; i < 1000; i++)
//            {
//                items.Add(new HeapItem(i));
//                heap.Add(items.Last());
//            }

//            int index = heap.IndexOf(items[645]);
//            items[645].Value = 14;
//            heap.Update(index);

//            HeapItem value = null;
//            while (heap.Count > 0)
//            {
//                var removed = heap.RemoveMax();
//                if (value != null)
//                    Assert.IsTrue(removed.Value <= value.Value);
//                value = removed;
//            }
//        }

//        private class HeapItem
//            :IComparable<HeapItem>
//        {
//            public int Value;

//            public HeapItem(int v)
//            {
//                Value = v;
//            }

//            public int CompareTo(HeapItem other)
//            {
//                return Value.CompareTo(other.Value);
//            }

//            public override string ToString()
//            {
//                return Value.ToString();
//            }
//        }

//        private void AssertHeap(MinMaxHeap_Accessor<int> heap)
//        {
//            for (int i = 0; i < heap.Count; i++)
//            {
//                if (MinMaxHeap_Accessor<int>.IsMinLevel(i))
//                {
//                    var errors = heap._heap.Skip(i).Where(item => !(item >= heap._heap[i])).ToArray();
//                    Assert.IsTrue(heap._heap.Skip(i).All(item => item >= heap._heap[i]));
//                }
//                else
//                {
//                    var errors = heap._heap.Skip(i).Where(item => !(item <= heap._heap[i])).ToArray();
//                    Assert.IsTrue(heap._heap.Skip(i).All(item => item <= heap._heap[i]));
//                }
//            }
//        }
//    }
//}
