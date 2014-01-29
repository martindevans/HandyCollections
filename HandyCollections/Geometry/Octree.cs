using System.Collections.Generic;
using System.Linq;

namespace HandyCollections.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Octree<T>
    {
        private readonly int _threshold;
        private readonly Node _root;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="threshold"></param>
        public Octree(Vector3 min, Vector3 max, int threshold)
        {
            _threshold = threshold;
            _root = new Node(min, max);
        }

        /// <summary>
        /// Insert an item into this octree
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="item"></param>
        public void Insert(BoundingBox bounds, T item)
        {
            var a = new Member {Bounds = bounds, Value = item};
            _root.Insert(a, _threshold);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public IEnumerable<T> Intersects(BoundingBox bounds)
        {
            return _root.Intersects(bounds).Select(a => a.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public IEnumerable<T> ContainedBy(BoundingBox bounds)
        {
            return _root.Intersects(bounds).Where(a => bounds.Contains(a.Bounds)).Select(a => a.Value);
        }

        private class Node
        {
            private readonly List<Member> _items = new List<Member>();

            private BoundingBox _bounds;
            private Node[] _children;

            public Node(Vector3 min, Vector3 max)
            {
                _bounds = new BoundingBox(min, max);
            }

            private void Split(int splitThreshold)
            {
                _children = new Node[8];
                int childIndex = 0;
                var min = _bounds.Min;
                var size = (_bounds.Max - _bounds.Min) / 2f;
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        for (int z = 0; z < 2; z++)
                        {
                            var positionOffset = size * new Vector3(x, y, z);
                            _children[childIndex++] = new Node(min + positionOffset, min + size + positionOffset);
                        }
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

            public IEnumerable<Member> Intersects(BoundingBox bounds)
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
                    foreach (var member in _children.Where(c => c._bounds.Intersects(bounds)).SelectMany(c => c.Intersects(bounds)))
                    {
                        yield return member;
                    }
                }
            }
        }

        private struct Member
        {
            public T Value;
            public BoundingBox Bounds;
        }
    }
}
