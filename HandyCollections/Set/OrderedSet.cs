using System.Collections.Generic;
using System.Linq;

namespace HandyCollections.Set
{
    /// <summary>
    /// A set which preserves the order in which items were added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrderedSet<T>
        : ISet<T>
    {
        #region fields
        private uint _next = 0;
        private readonly Dictionary<T, uint> _items;

        private IEnumerable<T> _enumerable; 
        #endregion

        #region constructor
        /// <summary>
        /// Create a new ordered set with the given equality comparer
        /// </summary>
        /// <param name="comparer"></param>
        public OrderedSet(IEqualityComparer<T> comparer)
        {
            _items = new Dictionary<T, uint>(comparer);

            _enumerable = _items
                .OrderBy(a => a.Value)
                .Select(a => a.Key);
        }

        /// <summary>
        /// Create a new ordered set with the default equality comparer
        /// </summary>
        public OrderedSet()
            : this(EqualityComparer<T>.Default)
        {
        }
        #endregion

        /// <summary>
        /// Add a new item to the set
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(T item)
        {
            if (_items.ContainsKey(item))
                return false;

            _items.Add(item, _next++);
            return true;
        }

        /// <summary>
        /// Remove all items in this set which are in the given set
        /// </summary>
        /// <param name="other"></param>
        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                Remove(item);
        }

        /// <summary>
        /// Modify this collection so that it only contains items which are in both collections
        /// </summary>
        /// <param name="other"></param>
        public void IntersectWith(IEnumerable<T> other)
        {
            foreach (var key in _items.Keys.Where(key => !other.Contains(key)).ToArray())
                _items.Remove(key);
        }

        /// <summary>
        /// Determine whether this set is a proper subset of given set
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (_items.Keys.Any(key => !other.Contains(key)))
                return false;

            return other.Count() > _items.Count;
        }

        /// <summary>
        /// Determines if this set is a proper superset of the given collection (this is a superset, and no merely equal)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var count = 0;
            foreach (var item in other)
            {
                count++;
                if (!_items.ContainsKey(item))
                    return false;
            }

            return count < _items.Count;
        }

        /// <summary>
        /// Determine if this set is a subset of the given collection
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _items.Keys.All(other.Contains);
        }

        /// <summary>
        /// Determine if this set is a superset of the given collection
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return other.All(_items.ContainsKey);
        }

        /// <summary>
        /// Determine if either collection contains an item which is also in the other collection
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Overlaps(IEnumerable<T> other)
        {
            //we'll put each item into the temp container and check the set
            return other.Any(item => _items.ContainsKey(item));
        }

        /// <summary>
        /// Determine if this set equals the given collection (contains the same items)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SetEquals(IEnumerable<T> other)
        {
            //Check that the set contains all the items in other
            var count = 0;
            foreach (var item in other)
            {
                count++;

                if (!_items.ContainsKey(item))
                    return false;
            }

            //If the count is the same then they are identical
            return count == _items.Count;
        }

        /// <summary>
        /// Modify this set to contain items which are in itself, or the other set, but *not* both
        /// </summary>
        /// <param name="other"></param>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var addItems = other.Where(a => !Contains(a)).ToArray();
            var removeItems = other.Where(Contains).ToArray();

            ExceptWith(removeItems);
            UnionWith(addItems);
        }

        /// <summary>
        /// Add all items to this collection
        /// </summary>
        /// <param name="other"></param>
        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                Add(item);
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Remove all items from this set
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _next = 0;
        }

        /// <summary>
        /// Determine if this set contains the given item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return _items.ContainsKey(item);
        }

        /// <summary>
        /// Copy items into the given array, starting at the given index (in the order the items were added)
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Number of items in this collection
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// False
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Remove the given item from this collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        /// <summary>
        /// Enumerate the items in the order they were added
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
