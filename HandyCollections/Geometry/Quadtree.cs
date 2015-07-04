using System;
using System.Collections.Generic;
using System.Linq;

namespace HandyCollections.Geometry
{
    public class Quadtree<T>
    {
        private readonly int _threshold;
        private readonly Node _root;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="threshold"></param>
        public Quadtree(Vector2 min, Vector2 max, int threshold)
        {
            _threshold = threshold;
            _root = new Node(min, max);
        }

        public void Insert(BoundingRectangle bounds, T item)
        {
            var a = new Member { Bounds = bounds, Value = item };
            _root.Insert(a, _threshold);
        }

        public IEnumerable<T> Intersects(BoundingRectangle bounds)
        {
            return _root.Intersects(bounds).Select(a => a.Value);
        }

        public IEnumerable<T> ContainedBy(BoundingRectangle bounds)
        {
            return _root.Intersects(bounds).Where(a => bounds.Contains(a.Bounds)).Select(a => a.Value);
        }

        public bool Remove(BoundingRectangle bounds, T item)
        {
            return _root.Remove(bounds, item);
        }

        private class Node
        {
            private readonly List<Member> _items = new List<Member>();

            private BoundingRectangle _bounds;
            private Node[] _children;

            public Node(Vector2 min, Vector2 max)
            {
                _bounds = new BoundingRectangle(min, max);
            }

            private void Split(int splitThreshold)
            {
                _children = new Node[4];
                int childIndex = 0;
                var min = _bounds.Min;
                var size = (_bounds.Max - _bounds.Min) / 2f;
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        var positionOffset = size * new Vector2(x, y);
                        _children[childIndex++] = new Node(min + positionOffset, min + size + positionOffset);
                    }
                }

                for (int i = _items.Count - 1; i >= 0; i--)
                {
                    var item = _items[i];

                    //Try to insert this item into each child (if successful, it's removed from this node)
                    foreach (Node child in _children)
                    {
                        if (child._bounds.Contains(item.Bounds))
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
                        if (child._bounds.Contains(m.Bounds))
                        {
                            child.Insert(m, splitThreshold);
                            return;
                        }
                    }

                    //Failed! Can't find a child to contain this, store it here instead
                    _items.Add(m);
                }
            }

            public IEnumerable<Member> Intersects(BoundingRectangle bounds)
            {
                //Select items in this node
                foreach (var member in _items)
                {
                    if (member.Bounds.Intersects(bounds))
                        yield return member;
                }

                //Select items in children
                if (_children != null)
                {
                    foreach (var child in _children)
                    {
                        foreach (var member in child.Intersects(bounds))
                            yield return member;
                    }
                }
            }

            public bool Remove(BoundingRectangle bounds, T item)
            {
                var pred = new Predicate<Member>(a => a.Value.Equals(item));

                return RemoveRecursive(bounds, pred);
            }

            private bool RemoveRecursive(BoundingRectangle bounds, Predicate<Member> predicate)
            {
                var index = _items.FindIndex(predicate);
                if (index != -1)
                {
                    _items.RemoveAt(index);
                    return true;
                }

                foreach (var child in _children)
                {
                    if (child.RemoveRecursive(bounds, predicate))
                        return true;
                }

                return false;
            }
        }

        private struct Member
        {
            public T Value;
            public BoundingRectangle Bounds;
        }
    }
}
