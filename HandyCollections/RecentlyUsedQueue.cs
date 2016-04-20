using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace HandyCollections
{
    /// <summary>
    /// A double ended queue providing Most recently used and least recently used data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RecentlyUsedQueue<T>
        :IEnumerable<T>
    {
        private readonly LinkedList<T> _list = new LinkedList<T>();

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => _list.Count;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_list != null);
        }

        /// <summary>
        /// 'Uses' the specified item, ie. moves/adds it to the Most recently used position
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True, if the item was newly added, otherwise false</returns>
        public bool Use(T item)
        {
            bool newlyAdded = false;

            var node = _list.Find(item);
            if (node == null)
            {
                node = new LinkedListNode<T>(item);
                newlyAdded = true;
            }
            else
                _list.Remove(node);

            _list.AddLast(node);

            return newlyAdded;
        }

        /// <summary>
        /// Gets the least recently used.
        /// </summary>
        /// <value>The least recently used.</value>
        public T LeastRecentlyUsed => _list.First.Value;

        /// <summary>
        /// Removes the least recently used.
        /// </summary>
        /// <returns>the item which was removed</returns>
        public T RemoveLeastRecentlyUsed()
        {
            var v = _list.First.Value;
            _list.RemoveFirst();
            return v;
        }

        /// <summary>
        /// Gets the most recently used.
        /// </summary>
        /// <value>The most recently used.</value>
        public T MostRecentlyUsed => _list.Last.Value;

        /// <summary>
        /// Removes the most recently used.
        /// </summary>
        /// <returns>the item which was removed</returns>
        public T RemoveMostRecentlyUsed()
        {
            var v = _list.Last.Value;
            _list.RemoveLast();
            return v;
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True; if anything was removed, otherwise false</returns>
        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator which goes from least to most recently used
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
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
