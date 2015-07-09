using HandyCollections.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HandyCollectionsTest
{
    [TestClass]
    public class OctreeTest
    {
        private readonly Octree<string> _octree = new Octree<string>(new BoundingBox(new Vector3(0), new Vector3(10)), 1);
            
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
    }
}
