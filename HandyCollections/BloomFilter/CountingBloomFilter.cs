using System;

namespace HandyCollections.BloomFilter
{
    /// <summary>
    /// A bloom filter. False positives are possible, false negatives are not. Removing items is possible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CountingBloomFilter<T>
    {
        internal readonly byte[] Array;
        /// <summary>
        /// The number of keys to use for this filter
        /// </summary>
        private readonly int _keyCount;

        /// <summary>
        /// A hash generation function
        /// </summary>
        public delegate int GenerateHash(T a);
        private readonly GenerateHash _hashGenerator;

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
        public double FalsePositiveRate => BloomFilter<T>.CalculateFalsePositiveRate(_keyCount, Array.Length, Count);

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
            Array = new byte[size];
            _keyCount = keys;
            _hashGenerator = hashgen;
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
            Array = new byte[size];
            _keyCount = keys;

            _hashGenerator = hashgen;
        }

        /// <summary>
        /// Adds the specified item to the filter
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if this item was already in the set</returns>
        public bool Add(T item)
        {
            bool b = true;
            int hash = _hashGenerator.Invoke(item);
            for (int i = 0; i < _keyCount; i++)
            {
                hash++;
                int index = BloomFilter<T>.GetIndex(hash, Array.Length);
                if (Array[index] == byte.MaxValue)
                {
                    //Rollback changes
                    for (int r = i - 1; r >= 0; r--)
                    {
                        hash--;
                        Array[BloomFilter<T>.GetIndex(hash, Array.Length)]--;
                    }
                    throw new OverflowException("Bloom filter overflowed");
                }
                if (Array[index]++ == 0)
                    b = false;
            }

            Count++;

            return b;
        }

        /// <summary>
        /// Removes the specified item from the filter
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if this item was successfully removed</returns>
        public bool Remove(T item)
        {
            int hash = _hashGenerator.Invoke(item);
            for (int i = 0; i < _keyCount; i++)
            {
                hash++;
                int index = BloomFilter<T>.GetIndex(hash, Array.Length);
                if (Array[index] == 0)
                {
                    //Rollback changes
                    for (int r = i - 1; r >= 0; r--)
                    {
                        hash--;
                        Array[BloomFilter<T>.GetIndex(hash, Array.Length)]++;
                    }
                    return false;
                }
                Array[index]--;
            }

            Count--;

            return true;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            for (int i = 0; i < Array.Length; i++)
                Array[i] = 0;
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
            int hash = _hashGenerator(item);
            for (int i = 0; i < _keyCount; i++)
            {
                hash++;
                if (Array[BloomFilter<T>.GetIndex(hash, Array.Length)] == 0)
                    return false;
            }
            return true;
        }
    }
}
