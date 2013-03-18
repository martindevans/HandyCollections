using System;
using System.Collections.Generic;

namespace HandyCollections
{
    /// <summary>
    /// A bloom filter. False positives are possible, false negatives are not. Removing items is possible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CountingBloomFilter<T>
    {
        byte[] array;
        /// <summary>
        /// The number of keys to use for this filter
        /// </summary>
        public readonly int KeyCount;

        /// <summary>
        /// A hash generation function
        /// </summary>
        public delegate int GenerateHash(T a);
        private GenerateHash hashGenerator = BloomFilter<T>.SystemHash;

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
                return Math.Pow(1 - Math.Exp(-KeyCount * Count / ((float)array.Length)), KeyCount);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="size">The size in bits</param>
        /// <param name="keys">The key count</param>
        public CountingBloomFilter(int size, int keys)
            : this(size, keys, BloomFilter<T>.SystemHash)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="estimatedsize">The estimated number of items to add to the filter</param>
        /// <param name="targetFalsePositiveRate">The target positive rate.</param>
        public CountingBloomFilter(int estimatedsize, float targetFalsePositiveRate)
            : this(estimatedsize, targetFalsePositiveRate, BloomFilter<T>.SystemHash)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountingBloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="size">The size of the filter in bytes</param>
        /// <param name="keys">The number of keys to use</param>
        /// <param name="hashgen">The hash generation function</param>
        public CountingBloomFilter(int size, int keys, GenerateHash hashgen)
        {
            array = new byte[size];
            KeyCount = keys;
            hashGenerator = hashgen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountingBloomFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="estimatedsize">The estimated number of members of the set</param>
        /// <param name="targetFalsePositiveRate">The target false positive rate when the estimated size is attained</param>
        /// <param name="hashgen">The hash generation function</param>
        public CountingBloomFilter(int estimatedsize, float targetFalsePositiveRate, GenerateHash hashgen)
        {
            int size = (int)(-(estimatedsize * Math.Log(targetFalsePositiveRate)) / 0.480453014f);
            int keys = (int)(0.7f * size / estimatedsize);
            array = new byte[size];
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
            int hash = hashGenerator.Invoke(item);
            for (int i = 0; i < KeyCount; i++)
            {
                unsafe
                {
                    hash++;
                    int index = BloomFilter<T>.GetIndex(hash, array.Length);
                    if (array[index] == byte.MaxValue)
                    {
                        for (int r = i - 1; r >= 0; r--)
                        {
                            hash--;
                            array[BloomFilter<T>.GetIndex(hash, array.Length)]--;
                        }
                        throw new OverflowException("Bloom filter overflowed");
                    }
                    if (array[index]++ == 0)
                        b = false;
                }
            }
            return b;
        }

        /// <summary>
        /// Adds the specified item to the filter
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if this item was already in the set</returns>
        public bool Remove(T item)
        {
            if (!Contains(item))
                throw new ArgumentException("Item is not in filter");

            Count--;

            int hash = hashGenerator.Invoke(item);
            int first = hash;
            for (int i = 0; i < KeyCount; i++)
            {
                unsafe
                {
                    hash++;
                    int index = BloomFilter<T>.GetIndex(hash, array.Length);
                    if (array[index] == 0)
                    {
                        for (int r = i - 1; r >= 0; r--)
                        {
                            hash--;
                            array[BloomFilter<T>.GetIndex(hash, array.Length)]++;
                        }
                        return false;
                    }
                    array[index]--;
                }
            }

            return true;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            for (int i = 0; i < array.Length; i++)
                array[i] = 0;
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
            int hash = hashGenerator(item);
            for (int i = 0; i < KeyCount; i++)
            {
                unsafe
                {
                    hash++;
                    if (array[BloomFilter<T>.GetIndex(hash, array.Length)] == 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Unions the specified filters
        /// </summary>
        /// <param name="s">The s.</param>
        public void Union(CountingBloomFilter<T> s)
        {
            if (s.KeyCount != this.KeyCount)
                throw new ArgumentException("Cannot union two filters with different key counts");
            s.UnionOnto(array);
        }

        private void UnionOnto(byte[] other)
        {
            if (other.Length != this.array.Length)
                throw new ArgumentException("Cannot union two filters with different sizes");
            for (int i = 0; i < other.Length; i++)
            {
                other[i] += array[i];
                if (other[i] + array[i] > byte.MaxValue)
                {
                    throw new NotImplementedException("Rollback and throw an overflow exception");
                }
            }
        }
    }
}
