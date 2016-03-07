using System;
using System.Collections;
using System.Collections.Generic;

namespace HandyCollections.Geometry
{
    public abstract class GeometricTree<TItem, TVector, TBound>
        : IEnumerable<KeyValuePair<TBound, TItem>>
        where TVector : struct
        where TBound : struct
    {
        private readonly int _threshold;
        private readonly Node _root;

        public TBound Bounds => _root.Bounds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="threshold"></param>
        protected GeometricTree(TBound bounds, int threshold)
        {
            _threshold = threshold;
            _root = new Node(this, bounds);
        }

        protected abstract bool Contains(TBound container, ref TBound contained);
        protected abstract bool Intersects(TBound a, ref TBound b);

        protected abstract TBound[] Split(TBound bound);

        #region add
        public void Insert(TBound bounds, TItem item)
        {
            var a = new Member { Bounds = bounds, Value = item };
            _root.Insert(a, _threshold);
        }
        #endregion

        #region query
        public IEnumerable<TItem> Intersects(TBound bounds)
        {
            foreach (var item in _root.Intersects(bounds))
                yield return item.Value;
        }

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
        public void Clear()
        {
            _root.Clear();
        }

        public bool Remove(TBound bounds, TItem item)
        {
            return _root.Remove(bounds, item);
        }

        public bool Remove(TBound bounds, Predicate<TItem> pred)
        {
            return _root.Remove(bounds, pred);
        }
        #endregion

        #region helper types
        private class Node
        {
            public readonly List<Member> Items = new List<Member>();
            public readonly TBound Bounds;
            public Node[] Children;

            private readonly GeometricTree<TItem, TVector, TBound> _tree;

            public Node(GeometricTree<TItem, TVector, TBound> tree, TBound bounds)
            {
                _tree = tree;
                Bounds = bounds;
            }

            private void Split(int splitThreshold)
            {
                var bounds = _tree.Split(Bounds);

                Children = new Node[bounds.Length];
                for (var i = 0; i < bounds.Length; i++)
                    Children[i] = new Node(_tree, bounds[i]);

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

                    if (Items.Count > splitThreshold)
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

                while (nodes.Count > 0)
                {
                    //Remove node
                    var n = nodes[nodes.Count - 1];
                    nodes.RemoveAt(nodes.Count - 1);

                    //Skip nodes we do not intersect
                    if (!_tree.Intersects(n.Bounds, ref bounds))
                        continue;

                    //yield items as appropriate
                    foreach (var member in n.Items)
                        if (_tree.Intersects(member.Bounds, ref bounds))
                            yield return member;

                    //push children onto stack to be checked
                    if (n.Children != null)
                        nodes.AddRange(n.Children);
                }
            }

            public bool Remove(TBound bounds, TItem item)
            {
                var pred = new Predicate<Member>(a => a.Value.Equals(item));

                return RemoveRecursive(bounds, pred, true);
            }

            public bool Remove(TBound bounds, Predicate<TItem> pred)
            {
                var predInner = new Predicate<Member>(a => pred(a.Value));

                return RemoveRecursive(bounds, predInner, false);
            }

            private bool RemoveRecursive(TBound bounds, Predicate<Member> predicate, bool removeSingle)
            {
                if (!_tree.Intersects(Bounds, ref bounds))
                    return false;

                bool removed = false;

                if (removeSingle)
                {
                    //We're removing a single item, so find it and exit as soon as we do
                    var index = Items.FindIndex(predicate);
                    if (index != -1)
                    {
                        Items.RemoveAt(index);
                        return true;
                    }
                }
                else
                {
                    //We're removing all predicate matches
                    removed = Items.RemoveAll(predicate) > 0;
                }

                if (Children != null)
                {
                    foreach (var child in Children)
                    {
                        if (child.RemoveRecursive(bounds, predicate, removeSingle))
                            removed = true;
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
        public IEnumerator<KeyValuePair<TBound, TItem>> GetEnumerator()
        {
            var nodes = new List<Node>() { _root };
            while (nodes.Count > 0)
            {
                var n = nodes[nodes.Count - 1];
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
