using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

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
        [Obsolete]
        public static IEnumerable<T> Append<T>(this IEnumerable<T> start, IEnumerable<T> end)
        {
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
        public static IEnumerable<T> Append<T>(this IEnumerable<T> start, params T[] end)
        {
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
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        /// <summary>
        /// Given a item => value function selects the highest value item from a set of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T MaxItem<T>(this IEnumerable<T> items, Func<T, float> value)
        {
            Contract.Requires(items != null);
            Contract.Requires(value != null);

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
        public static T MinItem<T>(this IEnumerable<T> items, Func<T, float> value)
        {
            Contract.Requires(items != null);
            Contract.Requires(value != null);

            return items.MaxItem(a => -value(a));
        }
    }
}
