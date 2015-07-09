using System;
using System.Collections.Generic;
using System.Linq;

namespace HandyCollections.Geometry
{
    public abstract class GeometricTree<TItem, TVector, TBound>
    {
        private readonly int _threshold;
        private readonly Node _root;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="threshold"></param>
        /// <param name="dimension"></param>
        protected GeometricTree(TBound bounds, int threshold)
        {
            _threshold = threshold;
            _root = new Node(this, bounds);
        }

        protected abstract bool Contains(TBound container, TBound contained);
        protected abstract bool Intersects(TBound a, TBound b);

        protected abstract TBound[] Split(TBound bound);

        public void Insert(TBound bounds, TItem item)
        {
            var a = new Member { Bounds = bounds, Value = item };
            _root.Insert(a, _threshold);
        }

        public IEnumerable<TItem> Intersects(TBound bounds)
        {
            return _root.Intersects(bounds).Select(a => a.Value);
        }

        public IEnumerable<TItem> ContainedBy(TBound bounds)
        {
            return _root.Intersects(bounds).Where(a => Contains(bounds, a.Bounds)).Select(a => a.Value);
        }

        public bool Remove(TBound bounds, TItem item)
        {
            return _root.Remove(bounds, item);
        }

        private class Node
        {
            private readonly List<Member> _items = new List<Member>();

            private readonly GeometricTree<TItem, TVector, TBound> _tree;
            private readonly TBound _bounds;
            private Node[] _children;

            public Node(GeometricTree<TItem, TVector, TBound> tree, TBound bounds)
            {
                _tree = tree;
                _bounds = bounds;
            }

            private void Split(int splitThreshold)
            {
                var bounds = _tree.Split(_bounds);

                _children = new Node[bounds.Length];
                for (int i = 0; i < bounds.Length; i++)
                    _children[i] = new Node(_tree, bounds[i]);

                for (int i = _items.Count - 1; i >= 0; i--)
                {
                    var item = _items[i];

                    //Try to insert this item into each child (if successful, it's removed from this node)
                    foreach (Node child in _children)
                    {
                        if (_tree.Contains(child._bounds, item.Bounds))
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
                        if (_tree.Contains(child._bounds, m.Bounds))
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
                Stack<Node> nodes = new Stack<Node>();
                nodes.Push(this);

                while (nodes.Count > 0)
                {
                    var n = nodes.Pop();

                    //Skip nodes we do not intersect
                    if (!_tree.Intersects(n._bounds, bounds))
                        continue;

                    //yield items as appropriate
                    foreach (var member in _items)
                        if (_tree.Intersects(member.Bounds, bounds))
                            yield return member;

                    //push children onto stack to be checked
                    if (n._children != null)
                        for (int i = 0; i < n._children.Length; i++)
                            nodes.Push(n._children[i]);
                }
            }

            public bool Remove(TBound bounds, TItem item)
            {
                var pred = new Predicate<Member>(a => a.Value.Equals(item));

                return RemoveRecursive(bounds, pred);
            }

            private bool RemoveRecursive(TBound bounds, Predicate<Member> predicate)
            {
                if (!_tree.Intersects(_bounds, bounds))
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
    }
}
