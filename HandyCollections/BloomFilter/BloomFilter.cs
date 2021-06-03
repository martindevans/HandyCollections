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
        :IBloomFilter<T>
    {
        private readonly BitArray _array;

        /// <summary>
        /// The amount of space this filter is using (in bytes)
        /// </summary>
        public int Size => _array.Length * 8;

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
        public double FalsePositiveRate => CalculateFalsePositiveRate(_keyCount, _array.Count, Count);

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
        public BloomFilter(int estimatedsize, double targetFalsePositiveRate)
        {
            var size = (int)Math.Ceiling(-(estimatedsize * Math.Log(targetFalsePositiveRate)) / 0.480453014f);
            var keys = (int)(0.7f * size / estimatedsize);
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
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            Count++;

            var b = true;
            var hash = item.GetHashCode();
            for (var i = 0; i < _keyCount; i++)
            {
                hash++;
                var ik = GetIndex(hash, _array.Length);
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
            if (item is null)
                return false;

            var hash = item.GetHashCode();
            for (var i = 0; i < _keyCount; i++)
            {
                hash++;
                if (!_array.Get(GetIndex(hash, _array.Length)))
                    return false;
            }
            return true;
        }

        #region static helpers
        internal static int GetIndex(int hash, int arrayLength)
        {
            var k = StaticRandomNumber.Random(unchecked((uint)hash), (uint)arrayLength);
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
            return a?.GetHashCode() ?? 0;
        }
        #endregion
    }
}
