using System;
using System.Diagnostics;
using System.Numerics;
using HandyCollections.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SwizzleMyVectors;
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
        public void OctreeQueryingWorks()
        {
            _octree.Insert(new BoundingBox(new Vector3(1), new Vector3(2)), "hello");
            _octree.Insert(new BoundingBox(new Vector3(8), new Vector3(9)), "world");

            Assert.AreEqual("hello", _octree.ContainedBy(new BoundingBox(new Vector3(0), new Vector3(3))).Single());
        }

        [TestMethod]
        public void InsertingRipplesDown()
        {
            _octree.Insert(new BoundingBox(new Vector3(1), new Vector3(2)), "hello");
            _octree.Insert(new BoundingBox(new Vector3(2), new Vector3(3)), "world");
            _octree.Insert(new BoundingBox(new Vector3(3), new Vector3(4)), "こにちは");

            Assert.AreEqual("こにちは", _octree.ContainedBy(new BoundingBox(new Vector3(2.5f), new Vector3(4.5f))).Single());
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
