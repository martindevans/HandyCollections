using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
            MinHeap<int> heap = new MinHeap<int>();

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
            MinHeap<int> heap = new MinHeap<int>();

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

            MinHeap<int> heap = new MinHeap<int>(100, Comparer<int>.Default);
            foreach (var number in numbers)
                heap.Add(number);
            while (heap.Count > 0)
                heap.RemoveMin();
        }
    }
}
