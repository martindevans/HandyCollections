using System;
using System.Collections.Generic;

namespace HandyCollections.Extensions
{
    /// <summary>
    /// A set of extensions to the IList interface
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Selects the index of the item from the list which would be in the given position if the list were sorted, using the default comparer.
        /// This will leave the list in an undefined order afterwards
        /// </summary>
        /// <param name="list">The list to select values from</param>
        /// <param name="position">Position to select</param>
        /// <returns></returns>
        public static int OrderSelect<T>(this IList<T> list, int position)
        {
            return list.OrderSelect<T>(Comparer<T>.Default, position);
        }

        /// <summary>
        /// Selects the index of the item from the list which would be in the given position if the list were sorted.
        /// This will leave the list in an undefined order afterwards
        /// </summary>
        /// <param name="list">The list to select values from</param>
        /// <param name="position">Position to select</param>
        /// <param name="comparer">The comparer to use</param>
        /// <returns></returns>
        public static int OrderSelect<T>(this IList<T> list, IComparer<T> comparer, int position)
        {
            if (position < 0)
                throw new ArgumentOutOfRangeException("position");
            if (position >= list.Count)
                throw new ArgumentOutOfRangeException("position");

            return list.QuickSelect<T>(comparer, position, 0, list.Count);
        }

        private static int QuickSelect<T>(this IList<T> list, IComparer<T> comparer, int position, int start, int length)
        {
            //Quickselec algorithm
            //http://www.ics.uci.edu/~eppstein/161/960125.html

            int pivotIndex = start + length / 2; //todo: need to pick a better pivot

            //partition list
            pivotIndex = list.Partition<T>(comparer, start, start + length - 1, pivotIndex);

            int lengthL1 = pivotIndex - start;
            const int lengthL2 = 1;
            int lengthL3 = start + length - pivotIndex - 1;

            if (position < lengthL1) //position is in L1
                return list.QuickSelect<T>(comparer, position, start, lengthL1);
            else if (position >= lengthL1 + lengthL2) //position is in L3
                return list.QuickSelect<T>(comparer, position - lengthL1 - lengthL2, start + lengthL1 + lengthL2, lengthL3);
            else //position must be the pivot
                return pivotIndex;
        }

        /// <summary>
        /// Partitions the list around a given pivot index using the default comparer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="left">The leftmost index of the sublist</param>
        /// <param name="right">The rightmost index of the sublist</param>
        /// <param name="pivotIndex">Index of the pivot.</param>
        /// <returns>the new index of the pivot element</returns>
        public static int Partition<T>(this IList<T> list, int left, int right, int pivotIndex)
        {
            return Partition<T>(list, Comparer<T>.Default, left, right, pivotIndex);
        }

        /// <summary>
        /// Partition an IList around a given pivot index
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">list to reorder</param>
        /// <param name="comparer">comparer to use</param>
        /// <param name="left">left index of the sublist to order</param>
        /// <param name="right">right index of the sublist to order</param>
        /// <param name="pivotIndex">the index of the pivot</param>
        /// <returns>the new index of the pivot</returns>
        public static int Partition<T>(this IList<T> list, IComparer<T> comparer, int left, int right, int pivotIndex)
        {
            var pivotValue = list[pivotIndex];
            list.Swap(pivotIndex, right);
            var storeIndex = left;
            for (var i = left; i < right; i++)
            {
                if (comparer.Compare(list[i], pivotValue) < 0)
                {
                    list.Swap(i, storeIndex);
                    storeIndex++;
                }
            }
            list.Swap(storeIndex, right);
            return storeIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int Partition<T>(this IList<T> list, Func<T, bool> predicate, int left, int right)
        {
            //Close in two indices until they overlap
            while (left != right) {

                //Sweep up left hand side, looking for something which needs swapping
                while (left < right && predicate(list[left]))
                    left++;

                //Sweep down right hand side looking for something which needs swapping
                while (right > left && !predicate(list[right]))
                    right--;

                //Swap them!
                if (left < right)
                    Swap(list, left, right);
            };

            return left;
        }

        /// <summary>
        /// Swap the items in the two given positions
        /// </summary>
        /// <typeparam name="T">Type of the list</typeparam>
        /// <param name="list">a list to swap the items inside</param>
        /// <param name="a">index of the first item</param>
        /// <param name="b">index of the second item</param>
        public static void Swap<T>(this IList<T> list, int a, int b)
        {
            T aa = list[a];
            list[a] = list[b];
            list[b] = aa;
        }
    }
}
