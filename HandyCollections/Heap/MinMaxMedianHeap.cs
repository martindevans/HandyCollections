//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace HandyCollections.Heap
//{
//    /// <summary>
//    /// a heap which allows O(1) extraction of minimum, maximum and median items. With insertion/deletion in O(logn) time
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class MinMaxMedianHeap<T>
//        : ICollection<T>
//    {
//        #region fields and properties
//        private readonly MinMaxHeap<T> _lesserOrEqual;
//        private readonly MinMaxHeap<T> _greaterOrEqual;

//        private readonly IComparer<T> _comparer = Comparer<T>.Default;
//        /// <summary>
//        /// The comparer to use for items in this collection. Changing this comparer will trigger a heapify operation
//        /// </summary>
//        public IComparer<T> Comparer
//        {
//            get
//            {
//                return _comparer;
//            }
//            set
//            {
//                throw new NotImplementedException("Not implemented, need to reorder all the heaps");
//            }
//        }

//        /// <summary>
//        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </summary>
//        /// <value></value>
//        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
//        public int Count
//        {
//            get
//            {
//                return _lesserOrEqual.Count + _greaterOrEqual.Count;
//            }
//        }
//        #endregion

//        #region constructors
//        /// <summary>
//        /// Initializes a new instance of the <see cref="MinMaxMedianHeap&lt;T&gt;"/> class.
//        /// </summary>
//        /// <param name="capacity">The initial capacity.</param>
//        public MinMaxMedianHeap(int capacity)
//            : this(Comparer<T>.Default, capacity)
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="MinMaxMedianHeap&lt;T&gt;"/> class.
//        /// </summary>
//        /// <param name="initialItems">The initial items.</param>
//        public MinMaxMedianHeap(T[] initialItems)
//            : this(Comparer<T>.Default, initialItems)
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="MinMaxMedianHeap&lt;T&gt;"/> class.
//        /// </summary>
//        public MinMaxMedianHeap()
//            : this(0)
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="MinMaxMedianHeap&lt;T&gt;"/> class.
//        /// </summary>
//        /// <param name="comparer">The comparer to use</param>
//        /// <param name="capacity">The initial capacity of the heap</param>
//        public MinMaxMedianHeap(Comparer<T> comparer, int capacity = 0)
//        {
//            _comparer = comparer;
//            _lesserOrEqual = new MinMaxHeap<T>(comparer, capacity / 2);
//            _greaterOrEqual = new MinMaxHeap<T>(comparer, capacity / 2);
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="MinMaxMedianHeap&lt;T&gt;"/> class.
//        /// </summary>
//        /// <param name="comparer">The comparer to use</param>
//        /// <param name="initialItems">The initial items to put into the heap</param>
//        public MinMaxMedianHeap(Comparer<T> comparer, T[] initialItems)
//            : this(comparer, initialItems.Length)
//        {
//            AddMany(initialItems);
//        }
//        #endregion

//        #region Add
//        /// <summary>
//        /// Adds the specified item to the heap
//        /// </summary>
//        /// <param name="item">item to add to the heap</param>
//        public void Add(T item)
//        {
//            if (Count == 0)
//            {
//                _lesserOrEqual.Add(item);
//            }
//            else
//            {
//                int comparision = _comparer.Compare(item, Median);
//                if (comparision < 0)
//                {
//                    _lesserOrEqual.Add(item);
//                }
//                else if (comparision > 0)
//                {
//                    _greaterOrEqual.Add(item);
//                }
//                else
//                {
//                    SelectSmallerHeap(_lesserOrEqual).Add(item);
//                }
//            }
//            Rebalance();
//        }

//        /// <summary>
//        /// Add many items to the heap
//        /// </summary>
//        /// <param name="a"></param>
//        public void AddMany(IEnumerable<T> a)
//        {
//            var sorted = a.OrderBy(i => i, _comparer).ToArray();

//            int mid = sorted.Length / 2;
//            _lesserOrEqual.AddMany(sorted, 0, mid);
//            _greaterOrEqual.AddMany(sorted, mid, sorted.Length - mid);
//        }
//        #endregion

//        #region Remove
//        /// <summary>
//        /// Removes the maximum item from the heap
//        /// </summary>
//        /// <returns></returns>
//        public T RemoveMax()
//        {
//            if (Count == 0)
//                throw new InvalidOperationException("Heap is empty");

//            T value = _greaterOrEqual.Count == 0 ? _lesserOrEqual.RemoveMax() : _greaterOrEqual.RemoveMax();

//            Rebalance();
//            return value;
//        }

//        /// <summary>
//        /// Removes the minimum item from the heap
//        /// </summary>
//        /// <returns></returns>
//        public T RemoveMin()
//        {
//            if (Count == 0)
//                throw new InvalidOperationException("Heap is empty");

//            T value = _lesserOrEqual.Count == 0 ? _greaterOrEqual.RemoveMin() : _lesserOrEqual.RemoveMin();

//            Rebalance();
//            return value;
//        }

//        /// <summary>
//        /// Removes the median item from the heap
//        /// </summary>
//        /// <returns></returns>
//        public T RemoveMedian()
//        {
//            if (Count == 0)
//                throw new InvalidOperationException("Heap is empty");

//            T value = _lesserOrEqual.RemoveMax();

//            Rebalance();
//            return value;
//        }

//        /// <summary>
//        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </summary>
//        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
//        public void Clear()
//        {
//            _lesserOrEqual.Clear();
//            _greaterOrEqual.Clear();
//        }

//        /// <summary>
//        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </summary>
//        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
//        /// <returns>
//        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
//        /// </returns>
//        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
//        public bool Remove(T item)
//        {
//            int comparison = Comparer.Compare(item, Median);

//            if (comparison <= 0)
//                return _lesserOrEqual.Remove(item);
//            else
//                return _greaterOrEqual.Remove(item);
//        }
//        #endregion

//        #region helpers
//        /// <summary>
//        /// Rebalances the two trees, so (lesser.count == greater.count) | (lesser.count == greater.count + 1)
//        /// </summary>
//        private void Rebalance()
//        {
//            while (_lesserOrEqual.Count != _greaterOrEqual.Count && _lesserOrEqual.Count < _greaterOrEqual.Count + 1)
//            {
//                //pop off greater, and push onto lessser
//                _lesserOrEqual.Add(_greaterOrEqual.RemoveMin());
//            }
//            while (_lesserOrEqual.Count > _greaterOrEqual.Count + 1)
//            {
//                //pop off lesser, and push onto greater
//                _greaterOrEqual.Add(_lesserOrEqual.RemoveMax());
//            }
//        }

//        /// <summary>
//        /// Selects the smaller heap.
//        /// </summary>
//        /// <param name="whenEqual">The heap to return when equal.</param>
//        /// <returns></returns>
//        private MinMaxHeap<T> SelectSmallerHeap(MinMaxHeap<T> whenEqual)
//        {
//            return (_lesserOrEqual.Count < _greaterOrEqual.Count ? _lesserOrEqual : (_lesserOrEqual.Count > _greaterOrEqual.Count ? _greaterOrEqual : whenEqual));
//        }
//        #endregion

//        #region peeking
//        /// <summary>
//        /// Gets the maximum item in the heap
//        /// </summary>
//        /// <value>The max.</value>
//        public T Maximum
//        {
//            get
//            {
//                if (_greaterOrEqual.Count == 0)
//                    return _lesserOrEqual.Maximum;
//                else
//                    return _greaterOrEqual.Maximum;
//            }
//        }

//        /// <summary>
//        /// Gets the minimum item in the heap
//        /// </summary>
//        /// <value>The min.</value>
//        public T Minimum
//        {
//            get
//            {
//                if (_lesserOrEqual.Count == 0)
//                    return _greaterOrEqual.Minimum;
//                else
//                    return _lesserOrEqual.Minimum;
//            }
//        }

//        /// <summary>
//        /// Gets the median item in the heap. If there are a even number of items, the smaller of the two is selected
//        /// </summary>
//        /// <value>The median.</value>
//        public T Median
//        {
//            get
//            {
//                return LowMedian;
//            }
//        }

//        /// <summary>
//        /// Gets the median, when there are an even number of items this selects the smaller of the two medians
//        /// </summary>
//        /// <value>The low median.</value>
//        public T LowMedian
//        {
//            get
//            {
//                return _lesserOrEqual.Maximum;
//            }
//        }

//        /// <summary>
//        /// Gets the median, when there are an even number of items this selects the larger of the two medians
//        /// </summary>
//        /// <value>The high median.</value>
//        public T HighMedian
//        {
//            get
//            {
//                if (_greaterOrEqual.Count == 0)
//                    return LowMedian;
//                if (_greaterOrEqual.Count < _lesserOrEqual.Count)
//                    return LowMedian;
//                return _greaterOrEqual.Minimum;
//            }
//        }

//        /// <summary>
//        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
//        /// </summary>
//        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
//        /// <returns>
//        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
//        /// </returns>
//        public bool Contains(T item)
//        {
//            int comparison = Comparer.Compare(item, Median);

//            if (comparison < 0)
//                return _lesserOrEqual.Contains(item);
//            else if (comparison > 0)
//                return _greaterOrEqual.Contains(item);
//            else
//                return _lesserOrEqual.Contains(item) || _greaterOrEqual.Contains(item);
//        }
//        #endregion

//        /// <summary>
//        /// Returns a <see cref="System.String"/> that represents this instance.
//        /// </summary>
//        /// <returns>
//        /// A <see cref="System.String"/> that represents this instance.
//        /// </returns>
//        public override string ToString()
//        {
//            return "Count = " + Count;
//        }

//        #region ICollection<T> Members
//        /// <summary>
//        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
//        /// </summary>
//        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
//        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
//        /// <exception cref="T:System.ArgumentNullException">
//        /// 	<paramref name="array"/> is null.</exception>
//        /// <exception cref="T:System.ArgumentOutOfRangeException">
//        /// 	<paramref name="arrayIndex"/> is less than 0.</exception>
//        /// <exception cref="T:System.ArgumentException">
//        /// 	<paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type T cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
//        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
//        {
//            if (array == null)
//                throw new ArgumentNullException("array");
//            if (arrayIndex < 0)
//                throw new ArgumentOutOfRangeException("arrayIndex");
//            if (array.Rank != 1)
//                throw new ArgumentException("Array is multidimensional!");
//            if (arrayIndex >= array.Length)
//                throw new ArgumentException("index > length of array");
//            if (array.Length - arrayIndex < Count)
//                throw new ArgumentException("Not enough space in given array");

//            T[] items = this.ToArray();

//            for (int i = 0; i < items.Length; i++)
//            {
//                array[arrayIndex + i] = items[i];
//            }
//        }

//        /// <summary>
//        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
//        /// </summary>
//        /// <value></value>
//        /// <returns>always returns false</returns>
//        bool ICollection<T>.IsReadOnly
//        {
//            get
//            {
//                return false;
//            }
//        }
//        #endregion

//        #region IEnumerable<T> Members
//        /// <summary>
//        /// Returns an enumerator that iterates through the collection.
//        /// </summary>
//        /// <returns>
//        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
//        /// </returns>
//        IEnumerator<T> IEnumerable<T>.GetEnumerator()
//        {
//            return _lesserOrEqual.Concat(_greaterOrEqual).GetEnumerator();
//        }
//        #endregion

//        #region IEnumerable Members
//        /// <summary>
//        /// Returns an enumerator that iterates through a collection.
//        /// </summary>
//        /// <returns>
//        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
//        /// </returns>
//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return (this as IEnumerable<T>).GetEnumerator();
//        }
//        #endregion
//    }
//}
