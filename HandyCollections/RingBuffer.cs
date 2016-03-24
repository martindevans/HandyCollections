using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace HandyCollections
{
    /// <summary>
    /// Stores the N most recently added items
    /// </summary>
    public class RingBuffer<T>
        : IEnumerable<T>
    {
        private readonly T[] _items;

        /// <summary>
        /// Indicates the number of items added to the collection and currently stored
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The size of this ring buffer
        /// </summary>
        public int Capacity => _items.Length;

        private int _end;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public T this[int index]
        {
            get
            {
                Contract.Requires<IndexOutOfRangeException>(index >= 0 && index < Count);

                if (Count < Capacity)
                    return _items[index];
                return _items[(_end + index) % _items.Length];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public RingBuffer(int size)
        {
            Contract.Requires<ArgumentOutOfRangeException>(size > 0);

            _items = new T[size];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _items[_end] = item;
            _end = (_end + 1) % _items.Length;

            if (Count < _items.Length)
                Count++;
        }

        /// <summary>
        /// Enumerate items in the ringbuffer (oldest to newest)
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
