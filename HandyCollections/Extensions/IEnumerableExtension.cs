using System.Collections.Generic;
using System.Linq;

namespace HandyCollections.Extensions
{
    /// <summary>
    /// Extensions to the IEnumerable interface
    /// </summary>
// ReSharper disable InconsistentNaming
    public static class IEnumerableExtension
// ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// enumerates the start and then the end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
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
            return Append(start, end as IEnumerable<T>);
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
    }
}
