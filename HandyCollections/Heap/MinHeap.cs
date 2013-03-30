using System;
using System.Collections.Generic;

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

        private Queue<int> _searchQueue = new Queue<int>();

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
            _heap.Add(item);
            BubbleUp(_heap.Count - 1);
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

        private T RemoveAt(int index)
        {
            var removed = _heap[index];

            _heap[index] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            TrickleDown(index);

            return removed;
        }

        private void TrickleDown(int index)
        {
            while (index < _heap.Count)
            {
                int smallestChildIndex = SmallestChildSmallerThan(index, _heap[index]);
                if (smallestChildIndex == -1)
                    break;

                Swap(smallestChildIndex, index);
                index = smallestChildIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void Update(int index)
        {
            Add(RemoveAt(index));
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
            bool isLeftChild = leftChildIndex < _heap.Count;

            int rightChildIndex = RightChild(i);
            bool isRightChild = rightChildIndex < _heap.Count;

            if (!isLeftChild)
                return -1;

            T leftChild = _heap[leftChildIndex];

            if (!isRightChild || IsLessThan(leftChild, _heap[rightChildIndex]))
            {
                if (IsLessThan(leftChild, item))
                    return leftChildIndex;
                else
                    return -1;
            }
            else
            {
                if (IsLessThan(_heap[rightChildIndex], item))
                    return rightChildIndex;
                else
                    return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            _searchQueue.Clear();
            _searchQueue.Enqueue(0);

            while (_searchQueue.Count > 0)
            {
                var index = _searchQueue.Dequeue();
                var comparison = _comparer.Compare(_heap[index], item);

                if (comparison == 0)
                    return index;
                if (comparison == -1)
                {
                    _searchQueue.Enqueue(LeftChild(index));
                    _searchQueue.Enqueue(RightChild(index));
                }
            }

            return -1;
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

