using System;
using System.Collections.Generic;
using HandyCollections.Heap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class MinHeapTest
    {
        [TestMethod]
        public void AddItemToMinHeapCorrectlyUpdatesMinimum()
        {
            IMinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            Assert.AreEqual(5, heap.Minimum);

            heap.Add(3);
            Assert.AreEqual(3, heap.Minimum);

            heap.Add(7);
            Assert.AreEqual(3, heap.Minimum);
        }

        [TestMethod]
        public void RemoveItemsFromMinHeapCorrectlyUpdatesMinimum()
        {
            IMinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(3);
            heap.Add(7);

            Assert.AreEqual(3, heap.RemoveMin());

            Assert.AreEqual(5, heap.Minimum);
        }

        [TestMethod]
        public void InsertLotsOfNumbersToMinHeap()
        {
            var numbers = new int[] {
                12,12,12,12,12,12,13,12,13,11,12,12,13,12,12,12,13,13,12,13,13,13,12,12,13,12,13,12,13,13,12,14,12,12,13,12,12,13,13,12,14,13,12,12,13,13,13,13,13,13,13,12,13,13,14,13,13,13,13,13,
            };

            IMinHeap<int> heap = new MinHeap<int>(100, Comparer<int>.Default);
            foreach (var number in numbers)
                heap.Add(number);
            while (heap.Count > 0)
                heap.RemoveMin();
        }

        [TestMethod]
        public void BulkInsertToMinHeap()
        {
            var values = new[] {
                1, 2, 5, 213, 25, 3, 2, 5, 3, 2, 45, 2, 5, 2, 2, 4, 6, 32, 75, 5, 47, 7, 4, 3, 5, 34, 4
            };

            IMinHeap<int> heap = new MinHeap<int>(100, Comparer<int>.Default);
            heap.Add(values);

            //Sort values and test that the heap returns them in the same order
            Array.Sort(values);
            foreach (var value in values)
                Assert.AreEqual(value, heap.RemoveMin());
        }
    }
}
