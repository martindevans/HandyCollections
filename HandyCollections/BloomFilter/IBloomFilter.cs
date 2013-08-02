using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandyCollections.BloomFilter
{
    /// <summary>
    /// A probabalistic data set, which may return false positives but never false negatives
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBloomFilter<T>
    {
        /// <summary>
        /// Adds an item to the set if it was not already present
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if the item was already in the set</returns>
        bool Add(T item);

        /// <summary>
        /// Checks if the set might contains this item (bounded by the false probability rate)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(T item);

        /// <summary>
        /// Clear this set
        /// </summary>
        void Clear();
    }
}
