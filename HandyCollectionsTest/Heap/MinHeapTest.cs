using System;
using System.Collections.Generic;
using System.Linq;
using HandyCollections.Heap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest.Heap
{
    [TestClass]
    public class MinHeapTest
    {
        private void AssertHeapIsInOrder<T>(IMinHeap<T> heap, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;

            var first = true;
            var min = default(T);

            while (heap.Count > 0)
            {
                var i = heap.RemoveMin();
                if (!first)
                    Assert.IsTrue(comparer.Compare(i, min) >= 0);

                min = i;
                first = false;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AssertThat_Construct_Throws_WithNegativeCapacity()
        {
            new MinHeap<int>(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AssertThat_Construct_Throws_WithNegativeCapacity_WithComparer()
        {
            new MinHeap<int>(-1, Comparer<int>.Default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AssertThat_Construct_Throws_WithNegativeCapacity_WithComparison()
        {
            new MinHeap<int>(-1, Comparer<int>.Default.Compare);
        }

        [TestMethod]
        public void AssertThat_Clear_EmptiesHeap()
        {
            var heap = new MinHeap<int>();
            heap.Add(1);
            heap.Add(3);
            heap.Add(2);

            Assert.AreEqual(3, heap.Count);
            heap.Clear();
            Assert.AreEqual(0, heap.Count);
        }

        [TestMethod]
        public void AssertThat_Add_UpdatesMinimum()
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
        public void AssertThat_Remove_UpdatesMinimum()
        {
            IMinHeap<int> heap = new MinHeap<int>();

            heap.Add(5);
            heap.Add(3);
            heap.Add(7);

            Assert.AreEqual(3, heap.RemoveMin());
            Assert.AreEqual(5, heap.RemoveMin());
            Assert.AreEqual(7, heap.RemoveMin());
        }

        [TestMethod]
        public void AssertThat_AddingRange_CorrectlyOrdersHeap()
        {
            const int itemCount = 5000;

            var r = new Random(12434);
            var numbers = new List<int>(itemCount);

            for (var i = 0; i < itemCount; i++)
            {
                var n = r.Next(-100, 100);
                numbers.Add(n);
            }

            IMinHeap<int> heap = new MinHeap<int>(itemCount, Comparer<int>.Default);
            heap.Add(numbers);

            while (heap.Count > 0)
            {
                Assert.AreEqual(heap.Count, numbers.Count);

                var min = numbers.Min();
                numbers.Remove(min);

                Assert.AreEqual(min, heap.RemoveMin());
            }
        }

        [TestMethod]
        public void AssertThat_AddingAndRemoving_UpdatesMinimum_WithManyValues()
        {
            var r = new Random(12434);
            var numbers = new List<int>(1000);
            IMinHeap<int> heap = new MinHeap<int>(1000, Comparer<int>.Default);

            for (var i = 0; i < 1000; i++)
            {
                var n = r.Next(-100, 100);
                numbers.Add(n);
                heap.Add(n);
            }

            while (heap.Count > 0)
            {
                Assert.AreEqual(heap.Count, numbers.Count);

                var min = numbers.Min();
                numbers.Remove(min);

                Assert.AreEqual(min, heap.RemoveMin());
            }
        }

        [TestMethod]
        public void AssertThat_MutatingItemUp_IsCorrectedByHeapify()
        {
            var item = new Item(1);

            //Create a heap with a mutable item in it
            var heap = new MinHeap<Item>();
            heap.Add(new[] {
                new Item(42),
                new Item(17),
                new Item(4),
                new Item(8),
                new Item(15),
                item,
                new Item(32),
                new Item(64),
                new Item(25),
                new Item(99),
            });

            //Mutate the item, and then call heapify to fix things
            item.Value = 1000;
            heap.Heapify();

            AssertHeapIsInOrder(heap);
        }

        [TestMethod]
        public void AssertThat_MutatingItemDown_IsCorrectedByHeapify()
        {
            var item = new Item(1000);

            //Create a heap with a mutable item in it
            var heap = new MinHeap<Item>();
            heap.Add(new[] {
                new Item(42),
                new Item(17),
                new Item(4),
                new Item(8),
                new Item(15),
                item,
                new Item(32),
                new Item(64),
                new Item(25),
                new Item(99),
            });

            //Mutate the item, and then call heapify to fix things
            item.Value = 1;
            heap.Heapify();

            AssertHeapIsInOrder(heap);
        }

        [TestMethod]
        public void AssertThat_MutatingItemUp_IsCorrectedByHeapify_WithHint()
        {
            var item = new Item(1);

            //Create a heap with a mutable item in it
            var heap = new MinHeap<Item>();
            heap.Add(new[] {
                new Item(42),
                new Item(17),
                new Item(4),
                new Item(8),
                new Item(15),
                item,
                new Item(32),
                new Item(64),
                new Item(25),
                new Item(99),
            });

            //Mutate the item, and then call heapify to fix things
            var index = heap.IndexOf(item);
            item.Value = 1000;
            heap.Heapify(index);

            AssertHeapIsInOrder(heap);
        }

        [TestMethod]
        public void AssertThat_MutatingItemDown_IsCorrectedByHeapify_WithHint()
        {
            var item = new Item(1000);

            //Create a heap with a mutable item in it
            var heap = new MinHeap<Item>();
            heap.Add(new[] {
                new Item(42),
                new Item(17),
                new Item(4),
                new Item(8),
                new Item(15),
                item,
                new Item(32),
                new Item(64),
                new Item(25),
                new Item(99),
            });

            //Mutate the item, and then call heapify to fix things
            var index = heap.IndexOf(item);
            item.Value = 1;
            heap.Heapify(index);

            AssertHeapIsInOrder(heap);
        }

        [TestMethod]
        public void AssertThat_RemoveAt_UpdatesMinimum()
        {
            var heap = new MinHeap<int> { 10, 14, 7, 11, 0 };

            Assert.AreEqual(0, heap.Minimum);
            heap.RemoveAt(0);
            Assert.AreEqual(7, heap.Minimum);
        }

        [TestMethod]
        public void AssertThat_RemoveAt_MaintainsHeapOrder()
        {
            var heap = new MinHeap<int> { 10, 14, 7, 11, 0 };

            Assert.AreEqual(0, heap.Minimum);
            heap.RemoveAt(3);
            Assert.AreEqual(0, heap.Minimum);

            AssertHeapIsInOrder(heap);
        }

        [TestMethod]
        public void AssertThat_EnumeratingHeap_GetsAllItemsInHeap()
        {
            var heap = new MinHeap<int> { 10, 14, 7, 11, 0 };

            var items = heap.ToArray();

            Assert.AreEqual(heap.Count, items.Length);
            Assert.IsTrue(items.Contains(10));
            Assert.IsTrue(items.Contains(14));
            Assert.IsTrue(items.Contains(7));
            Assert.IsTrue(items.Contains(11));
            Assert.IsTrue(items.Contains(0));
        }

        private class Item
            : IComparable<Item>
        {
            public int Value;

            public Item(int value)
            {
                Value = value;
            }

            public int CompareTo(Item other)
            {
                return Value.CompareTo(other.Value);
            }
        }
    }
}
