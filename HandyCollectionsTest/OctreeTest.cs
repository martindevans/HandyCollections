using System;
using System.Diagnostics;
using System.Numerics;
using HandyCollections.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SwizzleMyVectors.Geometry;

namespace HandyCollectionsTest
{
    [TestClass]
    public class OctreeTest
    {
        private readonly Octree<string> _octree = new Octree<string>(new BoundingBox(new Vector3(0), new Vector3(1000)), 1);
            
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void AssertThat_QueryingOctreeContainedBy_ReturnsItemsContainedInQueryBounds()
        {
            _octree.Insert(new BoundingBox(new Vector3(1), new Vector3(2)), "hello");
            _octree.Insert(new BoundingBox(new Vector3(8), new Vector3(9)), "world");
            Assert.AreEqual(2, _octree.Count);

            Assert.AreEqual("hello", _octree.ContainedBy(new BoundingBox(new Vector3(0), new Vector3(3))).Single());
        }

        [TestMethod]
        public void AssertThat_QueryingOctreeIntersects_ReturnsItemsIntersectingQueryBounds()
        {
            _octree.Insert(new BoundingBox(new Vector3(1), new Vector3(2)), "hello");
            _octree.Insert(new BoundingBox(new Vector3(2), new Vector3(3)), "world");
            _octree.Insert(new BoundingBox(new Vector3(3), new Vector3(4)), "こにちは");
            Assert.AreEqual(3, _octree.Count);

            var items = _octree.Intersects(new BoundingBox(new Vector3(2.5f), new Vector3(4.5f))).ToArray();
            Assert.AreEqual(2, items.Length);

            Assert.IsFalse(items.Contains("hello"));
            Assert.IsTrue(items.Contains("world"));
            Assert.IsTrue(items.Contains("こにちは"));
        }

        [TestMethod]
        public void AssertThat_RemovingFromOctreeByItem_PreventsItemsFromBeingReturnedInQuery()
        {
            //Add 2 items
            _octree.Insert(new BoundingBox(new Vector3(2), new Vector3(3)), "world");
            _octree.Insert(new BoundingBox(new Vector3(3), new Vector3(4)), "こにちは");
            Assert.AreEqual(2, _octree.Count);

            //Remove one of them
            _octree.Remove(new BoundingBox(Vector3.Zero, new Vector3(10)), "こにちは");
            Assert.AreEqual(1, _octree.Count);

            //This bounds would return *both* items if the removal was not there
            var items = _octree.Intersects(new BoundingBox(new Vector3(2.5f), new Vector3(4.5f))).ToArray();

            //Check that we didn't find the deleted item
            Assert.AreEqual(1, items.Length);
            Assert.IsTrue(items.Contains("world"));
            Assert.IsFalse(items.Contains("こにちは"));
        }

        [TestMethod]
        public void AssertThat_RemovingFromOctreeByPredicate_PreventsItemsFromBeingReturnedInQuery()
        {
            //Insert 3 items
            _octree.Insert(new BoundingBox(new Vector3(1), new Vector3(2)), "hello");
            _octree.Insert(new BoundingBox(new Vector3(2), new Vector3(3)), "world");
            _octree.Insert(new BoundingBox(new Vector3(3), new Vector3(4)), "こにちは");
            Assert.AreEqual(3, _octree.Count);

            //Remove all but one
            _octree.Remove(new BoundingBox(new Vector3(0), new Vector3(100)), a => a != "world");
            Assert.AreEqual(1, _octree.Count);

            var items = _octree.Intersects(new BoundingBox(new Vector3(2.5f), new Vector3(4.5f))).ToArray();

            Assert.AreEqual(1, items.Length);

            Assert.IsFalse(items.Contains("hello"));
            Assert.IsTrue(items.Contains("world"));
            Assert.IsFalse(items.Contains("こにちは"));
        }

        [TestMethod]
        public void AssertThat_EnumeratingQuadtree_ReturnsAllItems()
        {
            //Insert item out of bounds
            _octree.Insert(new BoundingBox(new Vector3(-1), new Vector3(-2)), "hello");
            Assert.AreEqual(1, _octree.Count);

            //Insert items in bounds
            _octree.Insert(new BoundingBox(new Vector3(20), new Vector3(30)), "world");
            _octree.Insert(new BoundingBox(new Vector3(300), new Vector3(400)), "こにちは");
            Assert.AreEqual(3, _octree.Count);

            //Enumerate and check we have all items
            var items = _octree.ToArray();
            Assert.AreEqual(3, items.Length);
        }

        [TestMethod]
        public void AssertThat_RemovingItemsOutsideBoundary_RemovesItems()
        {
            _octree.Insert(new BoundingBox(new Vector3(-3), new Vector3(-4)), "hello");
            _octree.Insert(new BoundingBox(new Vector3(20), new Vector3(30)), "world");
            Assert.AreEqual(2, _octree.Count);

            _octree.Remove(new BoundingBox(new Vector3(-10), new Vector3(-1)), "hello");
            Assert.AreEqual(1, _octree.Count);

            Assert.AreEqual(1, _octree.Count);
        }

        [TestMethod]
        public void StressTest()
        {
            Random r = new Random();
            Func<BoundingBox> randomBounds = () => {
                var min = new Vector3((float)r.NextDouble() * 999, (float) r.NextDouble() * 999, (float) r.NextDouble() * 999);
                var max = new Vector3((float) r.NextDouble() * (1000 - min.X), (float) r.NextDouble() * (1000 - min.Y), (float) r.NextDouble() * (1000 - min.Z)) + min;
                return new BoundingBox(min, max);
            };

            Stopwatch w = new Stopwatch();
            w.Start();
            const int COUNT = 100000;
            for (int i = 0; i < COUNT; i++)
                _octree.Insert(randomBounds(), "Hello");
            Console.WriteLine(w.Elapsed.TotalMilliseconds / COUNT + "ms");
        }
    }
}
