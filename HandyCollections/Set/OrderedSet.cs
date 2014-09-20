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
        private readonly HashSet<T> _set;
        private readonly LinkedList<T> _items = new LinkedList<T>();

        public OrderedSet(IEqualityComparer<T> comparer)
        {
            _set = new HashSet<T>(comparer);
        }

        public OrderedSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public bool Add(T item)
        {
            if (_set.Add(item))
            {
                _items.AddLast(item);
                return true;
            }
            return false;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (T item in other)
                Remove(item);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _set.IntersectWith(other);

            foreach (T item in _items.Where(item => !_set.Contains(item)))
                _items.Remove(item);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var addItems = other.Where(a => !_set.Contains(a)).ToArray();
            var removeItems = other.Where(a => _set.Contains(a)).ToArray();

            ExceptWith(removeItems);
            UnionWith(addItems);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (T item in other)
                Add(item);
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            _set.Clear();
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _set.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (_set.Remove(item))
            {
                _items.Remove(item);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
