using System.Linq;
using System.Numerics;
using HandyCollections.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwizzleMyVectors.Geometry;

namespace HandyCollectionsTest.Geometry
{
    [TestClass]
    public class QuadtreeTest
    {
        private readonly Quadtree<string> _tree = new Quadtree<string>(new BoundingRectangle(Vector2.Zero, new Vector2(100, 100)), 3, 3);

        [TestMethod]
        public void AssertThat_Intersects_FindsItemInBounds()
        {
            _tree.Insert(new BoundingRectangle(new Vector2(10, 10), new Vector2(20, 20)), "A");
            _tree.Insert(new BoundingRectangle(new Vector2(90, 90), new Vector2(80, 80)), "B");

            var results = _tree.Intersects(new BoundingRectangle(new Vector2(0, 0), new Vector2(20, 20))).ToArray();

            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("A", results.Single());
        }

        [TestMethod]
        public void AssertThat_Intersects_FindsItemOutOfBounds()
        {
            _tree.Insert(new BoundingRectangle(new Vector2(-10, -10), new Vector2(-5, -5)), "A");
            _tree.Insert(new BoundingRectangle(new Vector2(90, 90), new Vector2(80, 80)), "B");

            var results = _tree.Intersects(new BoundingRectangle(new Vector2(-15, -15), new Vector2(-5, -5))).ToArray();

            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("A", results.Single());
        }
    }
}
