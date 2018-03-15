using System;
using System.Collections.Generic;
using System.Linq;
using HandyCollections.Heap;
using JetBrains.Annotations;

namespace HandyCollections.Extensions
{
    /// <summary>
    /// Extensions to the IEnumerable interface
    /// </summary>
// ReSharper disable InconsistentNaming
    public static class IEnumerableExtensions
// ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// enumerates the start and then the end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        [Pure, Obsolete] public static IEnumerable<T> Append<T>([NotNull] this IEnumerable<T> start, [NotNull] IEnumerable<T> end)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));

            foreach (var item in start)
                yield return item;

            foreach (var item in end)
                yield return item;
        }

        /// <summary>
        /// Appends the given items onto this enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        [Pure, NotNull] public static IEnumerable<T> Append<T>([NotNull] this IEnumerable<T> start, [NotNull] params T[] end)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));

            return start.Concat(end);
        }

        /// <summary>
        /// Determines whether the specified enumerable is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        /// 	<c>true</c> if the specified enumerable is empty; otherwise, <c>false</c>.
        /// </returns>
        [Pure] public static bool IsEmpty<T>([NotNull] this IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            return !enumerable.Any();
        }

        /// <summary>
        /// Given a item => value function selects the highest value item from a set of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure, CanBeNull] public static T MaxItem<T>([NotNull] this IEnumerable<T> items, [NotNull] Func<T, float> value)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var bestScore = float.NegativeInfinity;
            var best = default(T);

            foreach (var item in items)
            {
                var s = value(item);
                if (s > bestScore)
                {
                    bestScore = s;
                    best = item;
                }
            }

            return best;
        }

        /// <summary>
        /// Given a item => value function selects the lowest value item from a set of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure, CanBeNull] public static T MinItem<T>([NotNull] this IEnumerable<T> items, [NotNull] Func<T, float> value)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (value == null) throw new ArgumentNullException(nameof(value));

            return items.MaxItem(a => -value(a));
        }

        /// <summary>
        /// Convert given enumeration of items to a MinHeap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IMinHeap<KeyValuePair<float, T>> ToMinHeap<T>(this IEnumerable<T> items, Func<T, float> key)
        {
            //Create a heap which order on the key of a KVP
            IMinHeap<KeyValuePair<float, T>> heap = new MinHeap<KeyValuePair<float, T>>((a, b) => a.Key.CompareTo(b.Key));

            //Add all the items in bulk
            heap.Add(items.Select(item => new KeyValuePair<float, T>(key(item), item)));

            return heap;
        }
    }
}
