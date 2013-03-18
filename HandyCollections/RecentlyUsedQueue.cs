using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandyCollections
{
    /// <summary>
    /// A double ended queue providing Most recently used and least recently used data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RecentlyUsedQueue<T>
        :IEnumerable<T>
    {
        private LinkedList<T> list = new LinkedList<T>();

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyUsedQueue&lt;T&gt;"/> class.
        /// </summary>
        public RecentlyUsedQueue()
        {

        }

        /// <summary>
        /// 'Uses' the specified item, ie. moves/adds it to the Most recently used position
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True, if the item was newly added, otherwise false</returns>
        public bool Use(T item)
        {
            bool newlyAdded = false;

            var node = list.Find(item);
            if (node == null)
            {
                node = new LinkedListNode<T>(item);
                newlyAdded = true;
            }
            else
                list.Remove(node);

            list.AddLast(node);

            return newlyAdded;
        }

        /// <summary>
        /// Gets the least recently used.
        /// </summary>
        /// <value>The least recently used.</value>
        public T LeastRecentlyUsed
        {
            get
            {
                return list.First.Value;
            }
        }

        /// <summary>
        /// Removes the least recently used.
        /// </summary>
        /// <returns>the item which was removed</returns>
        public T RemoveLeastRecentlyUsed()
        {
            var v = list.First.Value;
            list.RemoveFirst();
            return v;
        }

        /// <summary>
        /// Gets the most recently used.
        /// </summary>
        /// <value>The most recently used.</value>
        public T MostRecentlyUsed
        {
            get
            {
                return list.Last.Value;
            }
        }

        /// <summary>
        /// Removes the most recently used.
        /// </summary>
        /// <returns>the item which was removed</returns>
        public T RemoveMostRecentlyUsed()
        {
            var v = list.Last.Value;
            list.RemoveLast();
            return v;
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True; if anything was removed, otherwise false</returns>
        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator which goes from least to most recently used
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator which goes from least to most recently used
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<T>).GetEnumerator();
        }
    }
}
