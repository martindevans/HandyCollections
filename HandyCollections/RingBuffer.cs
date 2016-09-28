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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_items != null);
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

        public void Add(T[] items)
        {
            Add(new ArraySegment<T>(items));
        }

        public void Add(ArraySegment<T> items)
        {
            if (items.Count > Capacity)
            {
                //this single operation will overwrite the entire buffer
                //Reset buffer to empty and copy in last <capacity> items

                _end = Capacity;
                Count = Capacity;
                Array.Copy(items.Array, items.Offset + items.Count - Capacity, _items, 0, Capacity);
            }
            else
            {
                if (_end + items.Count > Capacity)
                {
                    // going to run off the end of the buffer;
                    // copy as much as we can then put the rest at the start of the buffer
                    var remainingSpace = Capacity - _end;
                    Array.Copy(items.Array, items.Offset, _items, _end, remainingSpace);
                    Array.Copy(items.Array, items.Offset + remainingSpace, _items, 0, items.Count - remainingSpace);
                    _end = (_end + items.Count) % _items.Length;
                }
                else
                {
                    // copy the data into the buffer
                    Array.Copy(items.Array, items.Offset, _items, _end, items.Count);
                    _end += items.Count;
                }

                Count = Math.Min(Count + items.Count, Capacity);
            }
        }

        /// <summary>
        /// Copy as much data as possible into the given array segment
        /// </summary>
        /// <param name="output"></param>
        /// <returns>A subsection of the given segment, which contains the data</returns>
        public ArraySegment<T> CopyTo(ArraySegment<T> output)
        {
            var count = Math.Min(Count, output.Count);
            var start = (_end + Capacity - Count) % Capacity;
            if (start + count < Capacity)
            {
                //We can copy all the data we need in a single operation (no wrapping)
                Array.Copy(_items, start, output.Array, output.Offset, count);
            }
            else
            {
                //Copying this much data wraps around, so we need 2 copies
                var cp = Capacity - start;
                Array.Copy(_items, start, output.Array, output.Offset, cp);
                Array.Copy(_items, 0, output.Array, output.Offset + cp, count - cp);
            }

            return new ArraySegment<T>(output.Array, output.Offset, count);
        }

        public void Clear()
        {
            Count = 0;
            _end = 0;
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
