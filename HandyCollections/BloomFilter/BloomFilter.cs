using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace HandyCollections
{
    /// <summary>
    /// A Bloom filter, supports adding but not removing, and never returns false negatives on containment queries
    /// http://en.wikipedia.org/wiki/Bloom_filter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BloomFilter<T>
    {
        private BitArray array;

        /// <summary>
        /// The number of keys generated for a given item
        /// </summary>
        public readonly int KeyCount;

        CountingBloomFilter<T>.GenerateHash hashGenerator = SystemHash;

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
            get
            {
                return Math.Pow(1 - Math.Exp(-KeyCount * Count / ((float)array.Count)), KeyCount);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="size">The size in bits</param>
        /// <param name="keys">The key count</param>
        public BloomFilter(int size, int keys)
            :this(size, keys, SystemHash)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="estimatedsize">The estimated number of items to add to the filter</param>
        /// <param name="targetFalsePositiveRate">The target positive rate.</param>
        public BloomFilter(int estimatedsize, float targetFalsePositiveRate)
            :this(estimatedsize, targetFalsePositiveRate, SystemHash)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="size">The size in bits</param>
        /// <param name="keys">The key count</param>
        /// <param name="hashgen">The hash generation function</param>
        public BloomFilter(int size, int keys, CountingBloomFilter<T>.GenerateHash hashgen)
        {
            array = new BitArray(size, false);
            KeyCount = keys;

            hashGenerator = hashgen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="estimatedsize">The estimated number of items in the filter</param>
        /// <param name="targetFalsePositiveRate">The target positive rate when the estimated size is attained</param>
        /// <param name="hashgen">The hash generation function</param>
        public BloomFilter(int estimatedsize, float targetFalsePositiveRate, CountingBloomFilter<T>.GenerateHash hashgen)
        {
            int size = (int)(-(estimatedsize * Math.Log(targetFalsePositiveRate)) / 0.480453014f);
            int keys = (int)(0.7f * size / estimatedsize);
            array = new BitArray(size, false);
            KeyCount = keys;

            hashGenerator = hashgen;
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
            for (int i = 0; i < KeyCount; i++)
            {
                unsafe
                {
                    hash++;
                    int ik = GetIndex(hash, array.Length);
                    if (!array.Get(ik))
                    {
                        b = false;
                        array.Set(ik, true);
                    }
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
            array.SetAll(false);
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
            for (int i = 0; i < KeyCount; i++)
            {
                unsafe
                {
                    hash++;
                    if (!array.Get(GetIndex(hash, array.Length)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Unions the specified filters
        /// </summary>
        /// <param name="s">The s.</param>
        public void Union(BloomFilter<T> s)
        {
            if (s.KeyCount != KeyCount)
                throw new ArgumentException("Cannot union two filters with different key counts");
            s.UnionOnto(array);
        }

        private void UnionOnto(BitArray other)
        {
            if (other.Count != this.array.Count)
                throw new ArgumentException("Cannot union two filters with different lengths");
            other.Or(this.array);
        }

        /// <summary>
        /// Intersections the specified filters
        /// </summary>
        /// <param name="s">The s.</param>
        public void Intersection(BloomFilter<T> s)
        {
            if (s.KeyCount != KeyCount)
                throw new ArgumentException("Cannot union two filters with different key counts");
            s.IntersectOnto(array);
        }

        private void IntersectOnto(BitArray other)
        {
            if (other.Count != this.array.Count)
                throw new ArgumentException("Cannot union two filters with different lengths");
            other.And(this.array);
        }

        #region static helpers
        internal static unsafe int GetIndex(int hash, int arrayLength)
        {
            uint k = StaticRandomNumber.Random(*((uint*)&hash), (uint)arrayLength);
            return (int)k;
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
