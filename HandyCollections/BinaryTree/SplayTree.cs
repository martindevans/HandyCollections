using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandyCollections.BinaryTree
{
    public class SplayTree<K, V>
        :BinaryTree<K, V>
    {
        public override Node Add(K key, V value)
        {
            return Splay(base.Add(key, value));
        }

        public override BinaryTree<K, V>.Node Find(K key)
        {
            var f = base.Find(key);

            Splay(f);

            return f;
        }

        private Node Splay(Node n)
        {
            while (n != Root)
                SplayStep(n);

            return n;
        }

        private void SplayStep(Node n)
        {
            if (n.IsRoot)
                throw new InvalidOperationException("Cannot perform splay step on root");

            if (n.Parent.IsRoot)
                Zig(n);
            else
            {
                if (n.IsLeftChild == n.Parent.IsLeftChild)
                    ZigZig(n);
                else
                    ZigZag(n);
            }
        }

        private void Zig(Node x)
        {
            Rotate(x.Parent, x.IsLeftChild);
        }

        private void ZigZig(Node n)
        {
            Zig(n.Parent);
            Zig(n);
        }

        private void ZigZag(Node n)
        {
            Zig(n);
            Zig(n);
        }
    }
}
