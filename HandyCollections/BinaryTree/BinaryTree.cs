using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace HandyCollections.BinaryTree
{
    /// <summary>
    /// A Binary search tree with support for tree rotations
    /// </summary>
    /// <typeparam name="TK">Type of keys</typeparam>
    /// <typeparam name="TV">Type of values</typeparam>
    public class BinaryTree<TK, TV>
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

        public IComparer<TK> Comparer { get; }
        #endregion

        #region constructors
        /// <summary>
        /// 
        /// </summary>
        public BinaryTree()
            :this(Comparer<TK>.Default)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        public BinaryTree([NotNull] IComparer<TK> comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
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
        [NotNull] public virtual Node Add(TK key, TV value)
        {
            var n = CreateNode(key, value);

            var v = FindParent(n.Key, out var duplicate);

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
        [CanBeNull] public TV Remove(TK key)
        {
            var node = FindParent(key, out var duplicate);

            if (!duplicate)
                return default(TV);

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
        private KeyValuePair<Node, bool> FindParent(TK key, out bool duplicate)
        {
            duplicate = false;
            var r = Root;

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
        public virtual Node Find(TK key)
        {
            var v = FindParent(key, out var duplicate);

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
        public void Rotate([NotNull] Node pivot, bool rotateRight)
        {
            var pivotParent = pivot.Parent;
            var parentLeftSide = pivotParent == null || ReferenceEquals(pivotParent.Left, pivot);

            var rotator = rotateRight ? pivot.Left : pivot.Right;
            //Node otherSubtree = rotateRight ? pivot.Right : pivot.Left;
            var insideSubtree = rotator == null ? null : rotateRight ? rotator.Right : rotator.Left;
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
        private bool IsLessThan(TK a, TK b)
        {
            return Comparer.Compare(a, b) < 0;
        }

        private bool IsEqual(TK a, TK b)
        {
            return Comparer.Compare(a, b) == 0;
        }

        [NotNull] private static Node CreateNode(TK key, TV value)
        {
            return new Node(key, value);
        }

        private static void SetChild([NotNull] Node parent, [CanBeNull] Node child, bool left)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

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
            public readonly TK Key;
            /// <summary>
            /// The value of this node
            /// </summary>
// ReSharper disable MemberCanBePrivate.Global
            public readonly TV Value;
// ReSharper restore MemberCanBePrivate.Global

            private Node _parent;
            /// <summary>
            /// Gets the parent element of this node
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public Node Parent
            {
                get => _parent;
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
                get => _left;
                protected internal set => SetChild(value, ref _left);
            }

            private Node _right;
            /// <summary>
            /// Gets the right child of this node
            /// </summary>
            public Node Right
            {
                get => _right;
                protected internal set => SetChild(value, ref _right);
            }

            private void SetChild([CanBeNull] Node value, [CanBeNull] ref Node field)
            {
                if (value?.Parent != null)
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
            public bool IsRoot => Parent == null;

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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            protected internal Node(TK key, TV value)
            {
                Key = key;
                Value = value;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "Node " + new KeyValuePair<TK, TV>(Key, Value);
            }
        }
    }
}
