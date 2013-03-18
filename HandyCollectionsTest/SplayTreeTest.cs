using HandyCollections.BinaryTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class SplayTreeTest
    {
        [TestMethod]
        public void Add()
        {
            SplayTree<int, string> tree = new SplayTree<int, string>();

            var a = tree.Add(10, "a");
            var b = tree.Add(5, "b");

            Assert.IsTrue(b == tree.Root);
            Assert.IsTrue(b.Right == a);

            var c = tree.Add(7, "c");

            Assert.IsTrue(c == tree.Root);
            Assert.IsTrue(c.Left == b);
            Assert.IsTrue(c.Right == a);

            var d = tree.Add(3, "d");

            Assert.IsTrue(d == tree.Root);
            Assert.IsTrue(d.Left == null);
            Assert.IsTrue(d.Right == b);
            Assert.IsTrue(b.Left == null);
            Assert.IsTrue(b.Right == c);
            Assert.IsTrue(c.Right == a);
            Assert.IsTrue(c.Left == null);
        }

        [TestMethod]
        public void Find()
        {
            SplayTree<int, string> tree = new SplayTree<int, string>();

            var a = tree.Add(5, "a");
            var b = tree.Add(3, "b");
            var c = tree.Add(1, "c");
            var d = tree.Add(2, "d");
            var e = tree.Add(10, "e");

            var found = tree.Find(5);
            Assert.IsTrue(found == a);
            Assert.IsTrue(tree.Root == a);
        }
    }
}
