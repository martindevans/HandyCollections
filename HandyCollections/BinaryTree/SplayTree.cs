using JetBrains.Annotations;

namespace HandyCollections.BinaryTree
{
    /// <summary>
    /// A binary tree which reorders itself to make more recently accessed items more efficient to access
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public class SplayTree<TK, TV>
        :BinaryTree<TK, TV>
    {
        /// <summary>
        /// Add a new items to this tree
        /// </summary>
        /// <param name="key">The key this node (used for finding)</param>
        /// <param name="value">The value stored in this node</param>
        /// <returns></returns>
        public override Node Add(TK key, TV value)
        {
            return Splay(base.Add(key, value));
        }

        /// <summary>
        /// Find the node with the given key (or null)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override Node Find(TK key)
        {
            var f = base.Find(key);

            Splay(f);

            return f;
        }

        private Node Splay(Node n)
        {
            while (n != Root)
            {
                if (n.Parent.IsRoot)
                {
                    Zig(n);
                }
                else
                {
                    if (n.IsLeftChild == n.Parent.IsLeftChild)
                        ZigZig(n);
                    else
                        ZigZag(n);
                }
            }

            return n;
        }

        private void Zig([NotNull] Node x)
        {
            Rotate(x.Parent, x.IsLeftChild);
        }

        private void ZigZig([NotNull] Node n)
        {
            Zig(n.Parent);
            Zig(n);
        }

        private void ZigZag([NotNull] Node n)
        {
            Zig(n);
            Zig(n);
        }
    }
}
