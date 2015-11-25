using System;
using System.Collections.Generic;

namespace HandyCollections.Heap
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MinHeap<T>
        : IMinHeap<T>
    {
        private readonly List<T> _heap;
        private readonly IComparer<T> _comparer;

        /// <summary>
        /// Get the number of items in this heap
        /// </summary>
        public int Count
        {
            get { return _heap.Count; }
        }

        /// <summary>
        /// Get the minimum value in this heap
        /// </summary>
        public T Minimum
        {
            get { return _heap[0]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MinHeap()
            : this(100, Comparer<T>.Default)
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

        public MinHeap(int capacity, Comparison<T> comparison)
            : this(capacity, Comparer<T>.Create(comparison))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(T item)
        {
            DebugAssertHeapProperty();

            _heap.Add(item);
            BubbleUp(_heap.Count - 1);

            DebugAssertHeapProperty();
        }

        /// <summary>
        /// Add a large number of items to the heap. This is more efficient that simply calling add on each item individually
        /// </summary>
        /// <param name="items"></param>
        public void Add(IEnumerable<T> items)
        {
            DebugAssertHeapProperty();

            _heap.AddRange(items);

            //todo: simply sorting the heap is cheating - and more expensive that using the proper heapify algorithm!
            _heap.Sort(_comparer);

            DebugAssertHeapProperty();

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

            DebugAssertHeapProperty();

            _heap[index] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0 && index < _heap.Count)
                BubbleUp(TrickleDown(index));

            DebugAssertHeapProperty();

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

        private void DebugAssertHeapProperty()
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
            return (i - 1) / 2;
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
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int IndexOf(Predicate<T> predicate)
        {
            return _heap.FindIndex(predicate);
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