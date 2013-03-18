using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HandyCollections.BinaryTree;

namespace HandyCollectionsTest
{
    [TestClass]
    public class BinaryTreeTest
    {
        [TestMethod]
        public void BuildTree()
        {
            BinaryTree<int, string> tree = new BinaryTree<int, string>();

            var a = tree.Add(3, "a");
            var b = tree.Add(2, "b");
            var c = tree.Add(1, "c");
            var d = tree.Add(4, "d");

            Assert.IsTrue(a.Parent == null);
            Assert.IsTrue(a == tree.Root);
            Assert.IsTrue(a.Left == b);
            Assert.IsTrue(b.Left == c);
            Assert.IsTrue(a.Right == d);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DuplicateKeysNotAllowed()
        {
            BinaryTree<int, string> tree = new BinaryTree<int, string>();

            tree.Add(3, "a");
            tree.Add(3, "a");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void SearchingEmptyTreeFails()
        {
            var tree = new BinaryTree<int, int>();
            tree.Find(1);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void SearchingForNonExistantKeyFails()
        {
            BinaryTree<int, string> tree = new BinaryTree<int, string>();

            var a = tree.Add(4, "a");
            var b = tree.Add(3, "b");
            var c = tree.Add(5, "c");
            var d = tree.Add(2, "d");
            var e = tree.Add(6, "e");
            var f = tree.Add(-1, "f");
            var g = tree.Add(10, "g");
            var h = tree.Add(44, "h");

            tree.Find(100);
        }

        [TestMethod]
        public void Find()
        {
            BinaryTree<int, string> tree = new BinaryTree<int, string>();

            var a = tree.Add(4, "a");
            var b = tree.Add(3, "b");
            var c = tree.Add(5, "c");
            var d = tree.Add(2, "d");
            var e = tree.Add(6, "e");
            var f = tree.Add(-1, "f");
            var g = tree.Add(10, "g");
            var h = tree.Add(44, "h");

            Assert.IsTrue(tree.Find(4) == a);
            Assert.IsTrue(tree.Find(3) == b);
            Assert.IsTrue(tree.Find(5) == c);
            Assert.IsTrue(tree.Find(2) == d);
            Assert.IsTrue(tree.Find(6) == e);
            Assert.IsTrue(tree.Find(-1) == f);
            Assert.IsTrue(tree.Find(10) == g);
            Assert.IsTrue(tree.Find(44) == h);
        }

        [TestMethod]
        public void Rotate()
        {
            BinaryTree<int, string> tree = new BinaryTree<int, string>();

            var root = tree.Add(100, "ROOT");

            var q = tree.Add(5, "Q");
            var p = tree.Add(3, "P");
            var c = tree.Add(6, "C");
            var a = tree.Add(2, "A");
            var b = tree.Add(4, "B");

            Action testLeftTree = () =>
                {
                    Assert.IsTrue(tree.Root == root);

                    Assert.IsTrue(root.Left == q);
                    Assert.IsTrue(root.Right == null);

                    Assert.IsTrue(q.Left == p);
                    Assert.IsTrue(q.Right == c);

                    Assert.IsTrue(c.Left == null);
                    Assert.IsTrue(c.Right == null);

                    Assert.IsTrue(p.Left == a);
                    Assert.IsTrue(p.Right == b);
                };
            testLeftTree();

            tree.Rotate(q, true);

            Assert.IsTrue(tree.Root == root);

            Assert.IsTrue(root.Left == p);
            Assert.IsTrue(root.Right == null);

            Assert.IsTrue(p.Left == a);
            Assert.IsTrue(p.Right == q);

            Assert.IsTrue(q.Left == b);
            Assert.IsTrue(q.Right == c);

            tree.Rotate(p, false);

            testLeftTree();
        }
    }
}
