using System;
using System.Collections.Generic;

namespace HandyCollections.Geometry
{
    public abstract class GeometricTree<TItem, TVector, TBound>
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
            private readonly List<Member> _items = new List<Member>();

            private readonly GeometricTree<TItem, TVector, TBound> _tree;
            public readonly TBound Bounds;
            private Node[] _children;

            public Node(GeometricTree<TItem, TVector, TBound> tree, TBound bounds)
            {
                _tree = tree;
                Bounds = bounds;
            }

            private void Split(int splitThreshold)
            {
                var bounds = _tree.Split(Bounds);

                _children = new Node[bounds.Length];
                for (var i = 0; i < bounds.Length; i++)
                    _children[i] = new Node(_tree, bounds[i]);

                for (var i = _items.Count - 1; i >= 0; i--)
                {
                    var item = _items[i];

                    //Try to insert this item into each child (if successful, it's removed from this node)
                    foreach (var child in _children)
                    {
                        var cb = child.Bounds;
                        if (_tree.Contains(cb, ref item.Bounds))
                        {
                            child.Insert(item, splitThreshold);
                            _items.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            public void Insert(Member m, int splitThreshold)
            {
                if (_children == null)
                {
                    _items.Add(m);

                    if (_items.Count > splitThreshold)
                        Split(splitThreshold);
                }
                else
                {
                    //Try to put this item into a child node
                    foreach (var child in _children)
                    {
                        var cb = child.Bounds;
                        if (_tree.Contains(cb, ref m.Bounds))
                        {
                            child.Insert(m, splitThreshold);
                            return;
                        }
                    }

                    //Failed! Can't find a child to contain this, store it here instead
                    _items.Add(m);
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
                    foreach (var member in n._items)
                        if (_tree.Intersects(member.Bounds, ref bounds))
                            yield return member;

                    //push children onto stack to be checked
                    if (n._children != null)
                        nodes.AddRange(n._children);
                }
            }

            public bool Remove(TBound bounds, TItem item)
            {
                var pred = new Predicate<Member>(a => a.Value.Equals(item));

                return RemoveRecursive(bounds, pred);
            }

            public bool Remove(TBound bounds, Predicate<TItem> pred)
            {
                var predInner = new Predicate<Member>(a => pred(a.Value));

                return RemoveRecursive(bounds, predInner);
            }

            private bool RemoveRecursive(TBound bounds, Predicate<Member> predicate)
            {
                if (!_tree.Intersects(Bounds, ref bounds))
                    return false;

                var index = _items.FindIndex(predicate);
                if (index != -1)
                {
                    _items.RemoveAt(index);
                    return true;
                }

                if (_children != null)
                {
                    foreach (var child in _children)
                    {
                        if (child.RemoveRecursive(bounds, predicate))
                            return true;
                    }
                }

                return false;
            }
        }

        private struct Member
        {
            public TItem Value;
            public TBound Bounds;
        }
        #endregion
    }
}
