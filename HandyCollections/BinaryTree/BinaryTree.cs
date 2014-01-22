using System;
using System.Collections.Generic;

namespace HandyCollections.BinaryTree
{
    /// <summary>
    /// A Binary search tree with support for tree rotations
    /// </summary>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    public class BinaryTree<K, V>
    {
        #region fields and properties
        /// <summary>
        /// Gets the root of this tree
        /// </summary>
        public Node Root
        {
            get;
            private set;
        }

        private readonly IComparer<K> _comparer;
        /// <summary>
        /// The comparer to use for items in this collection. Changing this comparer will trigger a heapify operation
        /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        public IComparer<K> Comparer
// ReSharper restore MemberCanBePrivate.Global
        {
            get
            {
                return _comparer;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// 
        /// </summary>
        public BinaryTree()
            :this(Comparer<K>.Default)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
// ReSharper disable MemberCanBePrivate.Global
        public BinaryTree(IComparer<K> comparer)
// ReSharper restore MemberCanBePrivate.Global
        {
            _comparer = comparer;
        }
        #endregion

        #region add/remove
        /// <summary>
        /// Add a node to the tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual Node Add(K key, V value)
        {
            Node n = CreateNode(key, value);

            bool duplicate;
            var v = FindParent(n.Key, out duplicate);

            if (duplicate)
                throw new ArgumentException("Duplicate keys not allowed");

            if (v.Key == null)
                return (Root = n);

            SetChild(v.Key, n, v.Value);

            return n;
        }

        /// <summary>
        /// Remove the node with the given key from this tree
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public V Remove(K key)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region search

        /// <summary>
        /// Finds the parent for inserting this key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="duplicate">indicates if a node with the same key was located</param>
        /// <returns></returns>
        private KeyValuePair<Node, bool> FindParent(K key, out bool duplicate)
        {
            duplicate = false;
            Node r = Root;

            if (r == null)
                return new KeyValuePair<Node, bool>(null, false);

            while (true)
            {
                if (IsLessThan(key, r.Key))
                {
                    if (r.Left == null)
                        return new KeyValuePair<Node, bool>(r, true);

                    r = r.Left;
                }
                else if (IsEqual(key, r.Key))
                {
                    duplicate = true;
                    return new KeyValuePair<Node, bool>(r, false);
                }
                else
                {
                    if (r.Right == null)
                        return new KeyValuePair<Node, bool>(r, false);

                    r = r.Right;
                }
            }
        }

        /// <summary>
        /// Find a node with the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual Node Find(K key)
        {
            bool duplicate;
            var v = FindParent(key, out duplicate);

            if (!duplicate)
                throw new KeyNotFoundException("No such key in this tree");

            return v.Key;
        }
        #endregion

        #region tree rotation
        /// <summary>
        /// Rotates around the pivot node
        /// </summary>
        /// <remarks>http://webdocs.cs.ualberta.ca/~holte/T26/tree-rotation.html</remarks>
        /// <param name="pivot"></param>
        /// <param name="rotateRight"></param>
        public void Rotate(Node pivot, bool rotateRight)
        {
            Node pivotParent = pivot.Parent;
            bool parentLeftSide = pivotParent == null || ReferenceEquals(pivotParent.Left, pivot);

            Node rotator = rotateRight ? pivot.Left : pivot.Right;
            //Node otherSubtree = rotateRight ? pivot.Right : pivot.Left;
            Node insideSubtree = rotator == null ? null : rotateRight ? rotator.Right : rotator.Left;
            //Node outsideSubtree = rotator == null ? null : rotateRight ? rotator.Left : rotator.Right;

            SetChild(pivot, null, rotateRight);
            if (pivotParent != null)
                SetChild(pivotParent, null, parentLeftSide);
            if (rotator != null)
                SetChild(rotator, null, !rotateRight);

            SetChild(pivot, insideSubtree, rotateRight);
            if (rotator != null)
                SetChild(rotator, pivot, !rotateRight);
            if (pivotParent != null)
                SetChild(pivotParent, rotator, parentLeftSide);
            else if (rotator != null)
                Root = rotator;
            else
                Root = pivot;
        }
        #endregion

        #region helpers
        private bool IsLessThan(K a, K b)
        {
            return Comparer.Compare(a, b) < 0;
        }

        private bool IsEqual(K a, K b)
        {
            return Comparer.Compare(a, b) == 0;
        }

        private static Node CreateNode(K key, V value)
        {
            return new Node(key, value);
        }

        private static void SetChild(Node parent, Node child, bool left)
        {
            if (left)
                parent.Left = child;
            else
                parent.Right = child;
        }
        #endregion

        /// <summary>
        /// A node in a binary tree
        /// </summary>
        public class Node
        {
            /// <summary>
            /// The key of this node
            /// </summary>
            public readonly K Key;
            /// <summary>
            /// The value of this node
            /// </summary>
// ReSharper disable MemberCanBePrivate.Global
            public readonly V Value;
// ReSharper restore MemberCanBePrivate.Global

            private Node _parent;
            /// <summary>
            /// Gets the parent element of this node
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public Node Parent
            {
                get
                {
                    return _parent;
                }
                private set
                {
                    if (_parent != null)
                    {
                        var p = _parent;
                        _parent = null;
                        if (ReferenceEquals(p.Left, this))
                            p.Left = null;
                        else if (ReferenceEquals(p.Right, this))
                            p.Right = null;
                        else
                            throw new InvalidOperationException("parent of this node does not count this node as it's child");
                    }

                    _parent = value;
                }
            }

            private Node _left;
            /// <summary>
            /// Gets the left child of this node
            /// </summary>
            public Node Left
            {
                get
                {
                    return _left;
                }
                protected internal set
                {
                    SetChild(value, ref _left);
                }
            }

            private Node _right;
            /// <summary>
            /// Gets the right child of this node
            /// </summary>
            public Node Right
            {
                get
                {
                    return _right;
                }
                protected internal set
                {
                    SetChild(value, ref _right);
                }
            }

            private void SetChild(Node value, ref Node field)
            {
                if (value != null && value.Parent != null)
                    throw new ArgumentException("Parent must be null");

                if (field != null)
                    field._parent = null;

                field = value;

                if (field != null)
                    field.Parent = this;
            }

            /// <summary>
            /// Indicates if this is the root node of the tree
            /// </summary>
            public bool IsRoot
            {
                get
                {
                    return Parent == null;
                }
            }

            /// <summary>
            /// Indicates if this is the left child of it's parent
            /// </summary>
            public bool IsLeftChild
            {
                get
                {
                    if (Parent == null)
                        return false;
                    return ReferenceEquals(Parent.Left, this);
                }
            }

            /// <summary>
            /// Indicates if this is the right child of it's parent
            /// </summary>
            public bool IsRightChild
            {
                get
                {
                    if (Parent == null)
                        return false;
                    return ReferenceEquals(Parent.Right, this);
                }
            }

            protected internal Node(K key, V value)
            {
                Key = key;
                Value = value;
            }

            public override string ToString()
            {
                return "Node " + new KeyValuePair<K, V>(Key, Value).ToString();
            }
        }
    }
}
