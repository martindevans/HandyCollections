using System;
using System.Collections;
using HandyCollections.RandomNumber;

namespace HandyCollections.BloomFilter
{
    /// <summary>
    /// A Bloom filter, supports adding but not removing, and never returns false negatives on containment queries
    /// http://en.wikipedia.org/wiki/Bloom_filter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BloomFilter<T>
    {
        private readonly BitArray _array;

        /// <summary>
        /// The amount of space this filter is using (in bytes)
        /// </summary>
        public int Size
        {
            get { return _array.Length * 8; }
        }

        /// <summary>
        /// The number of keys generated for a given item
        /// </summary>
        private readonly int _keyCount;

        /// <summary>
        /// Gets the number of items which have been added to this filter
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current false positive rate.
        /// </summary>
        /// <value>The false positive rate.</value>
        public double FalsePositiveRate
        {
            get { return CalculateFalsePositiveRate(_keyCount, _array.Count, Count); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="size">The size in bits</param>
        /// <param name="keys">The key count</param>
        public BloomFilter(int size, int keys)
        {
            _array = new BitArray(size, false);
            _keyCount = keys;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="estimatedsize">The estimated number of items to add to the filter</param>
        /// <param name="targetFalsePositiveRate">The target positive rate.</param>
        public BloomFilter(int estimatedsize, float targetFalsePositiveRate)
        {
            int size = (int)(-(estimatedsize * Math.Log(targetFalsePositiveRate)) / 0.480453014f);
            int keys = (int)(0.7f * size / estimatedsize);
            _array = new BitArray(size, false);
            _keyCount = keys;
        }

        /// <summary>
        /// Adds the specified item to the filter
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if this item was already in the set</returns>
        public bool Add(T item)
        {
            Count++;

            bool b = true;
            int hash = item.GetHashCode();
            for (int i = 0; i < _keyCount; i++)
            {
                hash++;
                int ik = GetIndex(hash, _array.Length);
                if (!_array.Get(ik))
                {
                    b = false;
                    _array.Set(ik, true);
                }
            }
            return b;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            _array.SetAll(false);
        }

        /// <summary>
        /// Determines whether this filter contains the specificed object, this will sometimes return false positives but never false negatives
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if the filter might contain the item; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            int hash = item.GetHashCode();
            for (int i = 0; i < _keyCount; i++)
            {
                hash++;
                if (!_array.Get(GetIndex(hash, _array.Length)))
                    return false;
            }
            return true;
        }

        #region static helpers
        internal static unsafe int GetIndex(int hash, int arrayLength)
        {
            uint k = StaticRandomNumber.Random(*((uint*)&hash), (uint)arrayLength);
            return (int)k;
        }

        internal static double CalculateFalsePositiveRate(int keyCount, int arrayCount, int count)
        {
            return Math.Pow(1 - Math.Exp(-keyCount * count / ((float)arrayCount)), keyCount);
        }

        /// <summary>
        /// Uses the system "GetHashFunction" method to hash an object
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns></returns>
        public static int SystemHash(T a)
        {
            return a.GetHashCode();
        }
        #endregion
    }
}
