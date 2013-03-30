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
    }
}
