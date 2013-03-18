using System;
using System.Collections.Generic;
using System.Linq;

namespace HandyCollections.Heap
{
    /// <summary>
    /// A heap which allows O(1) extraction of both minimum and maximum items, and O(logn) insertion/deletion
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MinMaxHeap<T>
        :ICollection<T>
    {
        #region fields and properties
        private readonly List<T> _heap;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        public int Count
        {
            get
            {
                return _heap.Count;
            }
        }

        private IComparer<T> _comparer = Comparer<T>.Default;
        /// <summary>
        /// The comparer to use for items in this collection. Changing this comparer will trigger a heapify operation
        /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        public IComparer<T> Comparer
// ReSharper restore MemberCanBePrivate.Global
        {
            get
            {
                return _comparer;
            }
            set
            {
                _comparer = value;
                Heapify();
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxHeap&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
// ReSharper disable MemberCanBePrivate.Global
        public MinMaxHeap(int capacity)
// ReSharper restore MemberCanBePrivate.Global
            :this(Comparer<T>.Default, capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxHeap&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="initialItems">The initial items.</param>
        public MinMaxHeap(IEnumerable<T> initialItems)
            :this(Comparer<T>.Default, initialItems)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxHeap&lt;T&gt;"/> class.
        /// </summary>
        public MinMaxHeap()
            :this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxHeap&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use</param>
        /// <param name="capacity">The initial capacity of the heap</param>
        public MinMaxHeap(Comparer<T> comparer, int capacity = 0)
        {
            _comparer = comparer;
            _heap = new List<T>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxHeap&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use</param>
        /// <param name="initialItems">The initial items to put into the heap</param>
// ReSharper disable MemberCanBePrivate.Global
        public MinMaxHeap(Comparer<T> comparer, IEnumerable<T> initialItems)
// ReSharper restore MemberCanBePrivate.Global
            :this(comparer)
        {
            AddMany(initialItems);
        }
        #endregion

        #region add
        /// <summary>
        /// Adds the specified item to the heap
        /// </summary>
        /// <param name="a">item to add to the heap</param>
        public void Add(T a)
        {
            _heap.Add(a);
            int myPos = _heap.Count - 1;

            BubbleUp(myPos);
        }

        /// <summary>
        /// adds all these Items to the heap and heapifies the heap (more efficient than adding each of the Items individually)
        /// </summary>
        /// <param name="a"></param>
// ReSharper disable MemberCanBePrivate.Global
        public void AddMany(IEnumerable<T> a)
// ReSharper restore MemberCanBePrivate.Global
        {
            _heap.AddRange(a);
            Heapify();
        }

        /// <summary>
        /// Adds the items to the heap and heapifies
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="start">The start index to take items from</param>
        /// <param name="length">The number of items to take</param>
        public void AddMany(IEnumerable<T> a, int start, int length)
        {
            _heap.AddRange(a.Skip(start).Take(length)); //Linq <3
            Heapify();
        }
        #endregion

        #region deletion
        /// <summary>
        /// Clears this heap.
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }

        /// <summary>
        /// Deletes the item with the largest key in the heap
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the heap is empty</exception>
        public T RemoveMax()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Heap is empty!");

            T value;

            if (_heap.Count == 1)
            {
                value = _heap[0];
                Clear();
            }
            else if (_heap.Count == 2)
            {
                value = _heap[1];
                _heap.RemoveAt(1);
            }
            else
            {
                int maxPos = MaxIndex();
                value = _heap[maxPos];

                int lastPos = _heap.Count - 1;
                if (maxPos == lastPos)
                    _heap.RemoveAt(lastPos);
                else
                {
                    _heap[maxPos] = _heap[lastPos];
                    _heap.RemoveAt(lastPos);
                    TrickleDown(maxPos);
                }
            }

            return value;
        }

        /// <summary>
        /// Deletes the item with the smallest key in the heap
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the heap is empty</exception>
        public T RemoveMin()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Heap is empty!");

            T value = _heap[0];

            if (_heap.Count == 1)
            {
                Clear();
            }
            else if (_heap.Count == 2)
            {
                _heap.RemoveAt(0);
            }
            else
            {
                _heap[0] = _heap[_heap.Count - 1];
                _heap.RemoveAt(_heap.Count - 1);
                TrickleDown(0);
            }

            return value;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;

            int lastPos = _heap.Count - 1;
            if (lastPos == index)
                _heap.RemoveAt(lastPos);
            else
            {
                _heap[index] = _heap[lastPos];
                _heap.RemoveAt(lastPos);
                TrickleDown(BubbleUp(index));
            }
            return true;
        }

        /// <summary>
        /// Removes several nodes from the maximum end of the heap
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public void RemoveMax(int count)
        {
            if (count > Count)
                throw new InvalidOperationException("Not enough items in heap to remove " + count + " objects");
            if (count == Count)
                Clear();
            else
                for (int i = 0; i < count; i++)
                    RemoveMax();
        }

        /// <summary>
        /// Removes several nodes from the minimum end of the heap
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public void RemoveMin(int count)
        {
            if (count > Count)
                throw new InvalidOperationException("Not enough items in heap to remove " + count + " objects");
            if (count == Count)
                Clear();
            else
                for (int i = 0; i < count; i++)
                    RemoveMin();
        }
        #endregion

        #region peeking
        /// <summary>
        /// finds the largest item in the heap
        /// </summary>
        /// <returns>value with maximal key</returns>
        public T Maximum
        {
            get
            {
                int i = MaxIndex();
                if (i < 0)
                    throw new InvalidOperationException("Heap is empty");
                return _heap[i];
            }
        }

        /// <summary>
        /// Finds the index of the max element
        /// </summary>
        /// <returns></returns>
        private int MaxIndex()
        {
            if (_heap.Count == 0)
                return -1;
            if (_heap.Count == 1)
                return 0;
            if (_heap.Count == 2)
                return 1;
            return (Comparer.Compare(_heap[1], _heap[2]) > 0 ? 1 : 2);
        }

        /// <summary>
        /// finds the smallest item in the heap
        /// </summary>
        /// <returns>value with minimal key</returns>
        public T Minimum
        {
            get
            {
                if (_heap.Count == 0)
                    throw new InvalidOperationException("Heap is empty");
                return _heap[0];
            }
        }

        /// <summary>
        /// Determines whether the heap contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the heap.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the heap; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }
        #endregion

        #region heapify
        /// <summary>
        /// Reorder the heap
        /// </summary>
        private void Heapify()
        {
            for (int i = _heap.Count / 2 - 1; i >= 0; i--)
                TrickleDown(i);
        }
        #endregion

        #region trickledown
        private void TrickleDown(int index)
        {
            if (IsMinLevel(index))
                TrickleDownMin(index);
            else
                TrickleDownMax(index);
        }

        private void TrickleDownMin(int index)
        {
            int m = IndexMinChildGrandchild(index);
            if (m <= -1)
                return;
            if (IsLessThan(_heap[m], _heap[index]))
            {
                if (m > (index + 1) * 2) //check if this is a grandchild
                {
                    //m is a grandchild
                    Swap(m, index);
                    if (IsGreaterThan(_heap[m], _heap[Parent(m)]))
                        Swap(m, Parent(m));
                    TrickleDownMin(m);
                }
                else
                {
                    //m is a child
                    Swap(m, index);
                    TrickleDownMin(index);
                }
            }
        }

        private void TrickleDownMax(int index)
        {
            int m = IndexMaxChildGrandchild(index);
            if (m <= -1)
                return;
            if (IsGreaterThan(_heap[m], _heap[index]))
            {
                if (m > (index + 1) * 2)
                {
                    //m is a grandchild
                    Swap(m, index);
                    if (IsLessThan(_heap[m], _heap[Parent(m)]))
                        Swap(m, Parent(m));
                    TrickleDownMax(m);
                }
                else
                {
                    //m is a child
                    Swap(m, index);
                    TrickleDownMax(index);
                }
            }
        }
        #endregion

        #region bubble up
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>the final resting index of the item</returns>
        private int BubbleUp(int index)
        {
            int parent = Parent(index);
            if (parent < 0)
                return index;
            if (IsMinLevel(index))
            {
                if (IsGreaterThan(_heap[index], _heap[parent]))
                {
                    Swap(index, parent);
                    return BubbleUpMax(parent);
                }
                else
                    return BubbleUpMin(index);
            }
            else
            {
                if (IsLessThan(_heap[index], _heap[parent]))
                {
                    Swap(index, parent);
                    return BubbleUpMin(parent);
                }
                else
                    return BubbleUpMax(index);
            }
        }

        private int BubbleUpMax(int index)
        {
            int grandparent = Parent(Parent(index));
            if (grandparent < 0)
                return index;
            if (IsGreaterThan(_heap[index], _heap[grandparent]))
            {
                Swap(index, grandparent);
                return BubbleUpMax(grandparent);
            }
            return index;
        }

        private int BubbleUpMin(int index)
        {
            int grandparent = Parent(Parent(index));
            if (grandparent < 0)
                return index;
            if (IsLessThan(_heap[index], _heap[grandparent]))
            {
                Swap(index, grandparent);
                return BubbleUpMin(grandparent);
            }
            return index;
        }
        #endregion

        #region helpers
        private bool IsGreaterThanOrEqualTo(T a, T b)
        {
            return Comparer.Compare(a, b) >= 0;
        }

        private bool IsLessThanOrEqualTo(T a, T b)
        {
            return Comparer.Compare(a, b) <= 0;
        }

        private int IndexOf(T item)
        {
            Queue<int> toSearch = new Queue<int>();
            toSearch.Enqueue(0);

            while (toSearch.Count > 0)
            {
                var index = toSearch.Dequeue();
                var value = _heap[index];
                if (value.Equals(item))
                    return index;

                bool isMin = IsMinLevel(index);
                if ((isMin && IsGreaterThanOrEqualTo(item, value)) || (!isMin && IsLessThanOrEqualTo(item, value)))
                {
                    toSearch.Enqueue(IndexLeftChild(index));
                    toSearch.Enqueue(IndexRightChild(index));
                }
            }

            return -1;
        }

        private static bool IsMinLevel(int index)
        {
            int level = (int)Math.Floor(Math.Log(index + 1, 2.0));
            return level % 2 == 0;
        }

        private int Parent(int m)
        {
            if (m <= 0)
                return -1;
            return (m - 1) / 2;
        }

        private int IndexRightChild(int index)
        {
            return (index + 1) * 2;
        }

        private int IndexLeftChild(int index)
        {
            return ((index + 1) * 2) - 1;
        }

        private int IndexMinChildGrandchild(int index)
        {
            int indexMin = -1;
            int a = IndexLeftChild(index);
            int b = IndexRightChild(index);
            int c = ((a + 1) * 2) - 1;
            int d = ((a + 1) * 2);
            int e = ((b + 1) * 2) - 1;
            int f = ((b + 1) * 2);

            if (a < _heap.Count)
                indexMin = a;
            if (b < _heap.Count && IsLessThan(_heap[b], _heap[indexMin]))
                indexMin = b;
            if (c < _heap.Count && IsLessThan(_heap[c], _heap[indexMin]))
                indexMin = c;
            if (d < _heap.Count && IsLessThan(_heap[d], _heap[indexMin]))
                indexMin = d;
            if (e < _heap.Count && IsLessThan(_heap[e], _heap[indexMin]))
                indexMin = e;
            if (f < _heap.Count && IsLessThan(_heap[f], _heap[indexMin]))
                indexMin = f;

            return indexMin;
        }

        private int IndexMaxChildGrandchild(int index)
        {
            int indexMax = -1;
            int a = index * 2 + 1;
            int b = index * 2 + 2;
            int c = a * 2 + 1;
            int d = a * 2 + 2;
            int e = b * 2 + 1;
            int f = b * 2 + 2;

            if (a < _heap.Count)
                indexMax = a;
            if (b < _heap.Count && IsGreaterThan(_heap[b], _heap[indexMax]))
                indexMax = b;
            if (c < _heap.Count && IsGreaterThan(_heap[c], _heap[indexMax]))
                indexMax = c;
            if (d < _heap.Count && IsGreaterThan(_heap[d], _heap[indexMax]))
                indexMax = d;
            if (e < _heap.Count && IsGreaterThan(_heap[e], _heap[indexMax]))
                indexMax = e;
            if (f < _heap.Count && IsGreaterThan(_heap[f], _heap[indexMax]))
                indexMax = f;

            return indexMax;
        }

        private bool IsLessThan(T a, T b)
        {
            return Comparer.Compare(a, b) < 0;
        }

        private bool IsGreaterThan(T a, T b)
        {
            return Comparer.Compare(a, b) > 0;
        }

        private void Swap(int a, int b)
        {
            T parent = _heap[a];
            _heap[a] = _heap[b];
            _heap[b] = parent;
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Count = " + Count;
        }

        #region ICollection<T> Members
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type T cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Rank != 1)
                throw new ArgumentException("Array is multidimensional!");
            if (arrayIndex >= array.Length)
                throw new ArgumentException("index > length of array");
            if (array.Length - arrayIndex < _heap.Count)
                throw new ArgumentException("Not enough space in given array");
            int upperIndex = _heap.Count + arrayIndex;
            for (int i = arrayIndex; i < upperIndex; i++)
            {
                array[i] = _heap[i - arrayIndex];
            }
        }

        /// <summary>
        /// always false
        /// </summary>
        /// <value></value>
        /// <returns>false.</returns>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _heap.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _heap.GetEnumerator();
        }
        #endregion
    }
}
