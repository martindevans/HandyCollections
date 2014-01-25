using System.Linq;
using HandyCollections;
using HandyCollections.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class OctreeTest
    {
        private readonly Octree<string> _octree = new Octree<string>(new Vector3(0), new Vector3(10), 1);
            
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
    }
}
