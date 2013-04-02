using System;
using System.Collections.Generic;
using System.IO;

namespace HandyCollections.Heap
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MinHeap<T>
    {
        private readonly List<T> _heap;
        private readonly IComparer<T> _comparer;

        /// <summary>
        /// 
        /// </summary>
        public int Count { get {return _heap.Count; }
    }

    /// <summary>
        /// 
        /// </summary>
        public T Minimum
        {
            get { return _heap[0]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MinHeap()
            :this(100, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        public MinHeap(int capacity, IComparer<T> comparer)
        {
            _heap = new List<T>(capacity);
            _comparer = comparer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(T item)
        {
            AssertHeapProperty();

            _heap.Add(item);
            BubbleUp(_heap.Count - 1);

            AssertHeapProperty();
        }

        private void BubbleUp(int index)
        {
            while (index > 0)
            {
                int parent = ParentIndex(index);
                if (IsLessThan(_heap[index], _heap[parent]))
                {
                    Swap(parent, index);
                    index = parent;
                }
                else
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T RemoveMin()
        {
            return RemoveAt(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public T RemoveAt(int index)
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Heap is empty");
            if (index < 0 || index > _heap.Count)
                throw new ArgumentOutOfRangeException("index");

            var removed = _heap[index];

            AssertHeapProperty();

            _heap[index] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0 && index < _heap.Count)
                BubbleUp(TrickleDown(index));

            AssertHeapProperty();

            return removed;
        }

        private int TrickleDown(int index)
        {
            if (index >= _heap.Count)
                throw new ArgumentException();

            int smallestChildIndex = SmallestChildSmallerThan(index, _heap[index]);
            if (smallestChildIndex == -1)
                return index;

            Swap(smallestChildIndex, index);
            return TrickleDown(smallestChildIndex);
        }

        private void AssertHeapProperty()
        {
#if DEBUG
            //for (int i = 0; i < _heap.Count; i++)
            //    if (IsLessThan(_heap[i], Minimum))
            //        throw new Exception("Heap property violated");
#endif
        }

        private bool IsLessThan(T a, T b)
        {
            return _comparer.Compare(a, b) < 0;
        }

        private bool IsEqualTo(T a, T b)
        {
            return _comparer.Compare(a, b) == 0;
        }

        private static int ParentIndex(int i)
        {
            return (i - 1 ) / 2;
        }

        private void Swap(int a, int b)
        {
            T temp = _heap[a];
            _heap[a] = _heap[b];
            _heap[b] = temp;
        }

        private int LeftChild(int i)
        {
            return 2 * i + 1;
        }

        private int RightChild(int i)
        {
            return 2 * i + 2;
        }

        private int SmallestChildSmallerThan(int i, T item)
        {
            int leftChildIndex = LeftChild(i);

            int rightChildIndex = RightChild(i);

            int smallest = -1;
            if (leftChildIndex < _heap.Count)
                smallest = leftChildIndex;
            if (rightChildIndex < _heap.Count && IsLessThan(_heap[rightChildIndex], _heap[leftChildIndex]))
                smallest = rightChildIndex;

            if (smallest > -1 && IsLessThan(_heap[smallest], item))
                return smallest;

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return _heap.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }
    }
}

