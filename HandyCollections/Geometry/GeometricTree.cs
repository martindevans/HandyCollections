using System;
using System.Collections;
using System.Collections.Generic;

namespace HandyCollections.Geometry
{
    /// <summary>
    /// Base class for trees with partition space
    /// </summary>
    /// <typeparam name="TItem">Items in the tree</typeparam>
    /// <typeparam name="TVector">Type of vectors</typeparam>
    /// <typeparam name="TBound">Type of bounds (rectangle or equivalent)</typeparam>
    public abstract class GeometricTree<TItem, TVector, TBound>
        : IEnumerable<KeyValuePair<TBound, TItem>>
        where TVector : struct
        where TBound : struct
    {
        private readonly int _threshold;
        private readonly Node _root;

        /// <summary>
        /// The bounds of the root of this tree (inserting outside the root bounds is possible, but inefficient)
        /// </summary>
        public TBound Bounds => _root.Bounds;

        /// <summary>
        /// The number of items in the entire tree
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="threshold"></param>
        /// <param name="maxDepth"></param>
        protected GeometricTree(TBound bounds, int threshold, int maxDepth)
        {
            _threshold = threshold;
            _root = new Node(this, bounds, 0, maxDepth);
        }

        /// <summary>
        /// Check if the given bound is contained entirely within the other bound
        /// </summary>
        /// <param name="container"></param>
        /// <param name="contained"></param>
        /// <returns></returns>
        protected abstract bool Contains(TBound container, ref TBound contained);

        /// <summary>
        /// Check if the given bounds intersects the other bound
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected abstract bool Intersects(TBound a, ref TBound b);

        /// <summary>
        /// Split the given bound is even parts
        /// </summary>
        /// <param name="bound"></param>
        /// <returns></returns>
        protected abstract TBound[] Split(TBound bound);

        #region add
        /// <summary>
        /// Insert a new item into the tree
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="item"></param>
        public void Insert(TBound bounds, TItem item)
        {
            var a = new Member { Bounds = bounds, Value = item };
            _root.Insert(a, _threshold);

            Count++;
        }
        #endregion

        #region query
        /// <summary>
        /// Find all items which intersect the given bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public IEnumerable<TItem> Intersects(TBound bounds)
        {
            foreach (var item in _root.Intersects(bounds))
                yield return item.Value;
        }

        /// <summary>
        /// Find all items which are completely contained byhe given bound
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public IEnumerable<TItem> ContainedBy(TBound bounds)
        {
            foreach (var item in _root.Intersects(bounds))
            {
                var b = item.Bounds;
                if (Contains(bounds, ref b))
                    yield return item.Value;
            }
        }
        #endregion

        #region remove
        /// <summary>
        /// Remove all items from the tree
        /// </summary>
        public void Clear()
        {
            _root.Clear();
            Count = 0;
        }

        /// <summary>
        /// Remove the specific item from the tree
        /// </summary>
        /// <param name="bounds">Hint used to find the item</param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TBound bounds, TItem item)
        {
            int count = _root.Remove(bounds, item);
            Count -= count;
            return count > 0;
        }

        /// <summary>
        /// Remove *all* items which match the specified predicate which intersect the given bound
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="pred"></param>
        /// <returns></returns>
        public bool Remove(TBound bounds, Predicate<TItem> pred)
        {
            int count = _root.Remove(bounds, pred);
            Count -= count;
            return count > 0;
        }
        #endregion

        #region helper types
        private class Node
        {
            public readonly List<Member> Items = new();
            public readonly TBound Bounds;
            public Node[]? Children;

            private readonly int _depth;
            private readonly int _maxDepth;

            private readonly GeometricTree<TItem, TVector, TBound> _tree;

            public Node(GeometricTree<TItem, TVector, TBound> tree, TBound bounds, int depth, int maxDepth)
            {
                _tree = tree;
                _depth = depth;
                _maxDepth = maxDepth;

                Bounds = bounds;
            }

            private void Split(int splitThreshold)
            {
                var bounds = _tree.Split(Bounds);

                Children = new Node[bounds.Length];
                for (var i = 0; i < bounds.Length; i++)
                    Children[i] = new Node(_tree, bounds[i], _depth + 1, _maxDepth);

                for (var i = Items.Count - 1; i >= 0; i--)
                {
                    var item = Items[i];

                    //Try to insert this item into each child (if successful, it's removed from this node)
                    foreach (var child in Children)
                    {
                        var cb = child.Bounds;
                        if (_tree.Contains(cb, ref item.Bounds))
                        {
                            child.Insert(item, splitThreshold);
                            Items.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            public void Insert(Member m, int splitThreshold)
            {
                if (Children == null)
                {
                    Items.Add(m);

                    if (Items.Count > splitThreshold && _depth < _maxDepth)
                        Split(splitThreshold);
                }
                else
                {
                    //Try to put this item into a child node
                    foreach (var child in Children)
                    {
                        var cb = child.Bounds;
                        if (_tree.Contains(cb, ref m.Bounds))
                        {
                            child.Insert(m, splitThreshold);
                            return;
                        }
                    }

                    //Failed! Can't find a child to contain this, store it here instead
                    Items.Add(m);
                }
            }

            public IEnumerable<Member> Intersects(TBound bounds)
            {
                var nodes = new List<Node>(50) { this };

                var root = true;
                while (nodes.Count > 0)
                {
                    //Remove node
                    var n = nodes[^1];
                    nodes.RemoveAt(nodes.Count - 1);

                    //Skip nodes we do not intersect (unless this is the root, in which case we always want to check it)
                    if (!root && !_tree.Intersects(n.Bounds, ref bounds))
                        continue;

                    //yield items as appropriate
                    foreach (var member in n.Items)
                        if (_tree.Intersects(member.Bounds, ref bounds))
                            yield return member;

                    //push children onto stack to be checked
                    if (n.Children != null)
                        nodes.AddRange(n.Children);

                    root = false;
                }
            }

            public int Remove(TBound bounds, TItem item)
            {
                var pred = new Predicate<Member>(a => a.Value!.Equals(item));

                return RemoveRecursive(bounds, pred, true);
            }

            public int Remove(TBound bounds, Predicate<TItem> pred)
            {
                var predInner = new Predicate<Member>(a => pred(a.Value));

                return RemoveRecursive(bounds, predInner, false);
            }

            private int RemoveRecursive(TBound bounds, Predicate<Member> predicate, bool removeSingle)
            {
                //We want to skip this node if the boundary doesn't intersect it... unless it's the root which can contain items outside it's boundary!
                if (!_tree.Intersects(Bounds, ref bounds) && !_tree._root.Equals(this))
                    return 0;

                int removed = 0;

                //Either we're removing the first item which matches the predicate, or all items which match
                if (removeSingle)
                {
                    // Find single item and remove it, then exit instantly
                    var index = Items.FindIndex(predicate);
                    if (index != -1)
                    {
                        Items.RemoveAt(index);
                        return 1;
                    }
                }
                else
                {
                    //We're removing all predicate matches
                    removed += Items.RemoveAll(predicate);
                }

                //Remove items from children (and aggregate total removed count)
                if (Children != null)
                {
                    foreach (var child in Children)
                    {
                        removed += child.RemoveRecursive(bounds, predicate, removeSingle);

                        //Early exit if we can
                        if (removed > 0 && removeSingle)
                            return removed;
                    }
                }

                return removed;
            }

            public void Clear()
            {
                Children = null;

                Items.Clear();
                Items.Capacity = 4;
            }
        }

        private struct Member
        {
            public TItem Value;
            public TBound Bounds;
        }
        #endregion

        #region enumeration
        /// <summary>
        /// Enumerate all items in the tree
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TBound, TItem>> GetEnumerator()
        {
            var nodes = new List<Node>() { _root };
            while (nodes.Count > 0)
            {
                var n = nodes[^1];
                nodes.RemoveAt(nodes.Count - 1);
                if (n == null)
                    continue;

                foreach (var item in n.Items)
                    yield return new KeyValuePair<TBound, TItem>(item.Bounds, item.Value);

                if (n.Children != null)
                    nodes.AddRange(n.Children);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
