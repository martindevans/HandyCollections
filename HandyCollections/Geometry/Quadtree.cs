using System.Numerics;
using SwizzleMyVectors.Geometry;

namespace HandyCollections.Geometry
{
    /// <summary>
    /// A 2D  Partitioning Tree
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class Quadtree<TItem>
        : GeometricTree<TItem, Vector2, BoundingRectangle>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="threshold"></param>
        /// <param name="maxDepth"></param>
        public Quadtree(BoundingRectangle bounds, int threshold, int maxDepth)
            : base(bounds, threshold, maxDepth)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="contained"></param>
        /// <returns></returns>
        protected override bool Contains(BoundingRectangle container, ref BoundingRectangle contained)
        {
            return container.Contains(contained);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override bool Intersects(BoundingRectangle a, ref BoundingRectangle b)
        {
            return a.Intersects(b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <returns></returns>
        protected override BoundingRectangle[] Split(BoundingRectangle bound)
        {
            var bounds = new BoundingRectangle[4];
            var min = bound.Min;
            var size = (bound.Max - bound.Min) / 2f;

            var i = 0;
            for (var x = 0; x < 2; x++)
            {
                for (var y = 0; y < 2; y++)
                {
                    var positionOffset = size * new Vector2(x, y);
                    bounds[i++] = new BoundingRectangle(min + positionOffset, min + size + positionOffset);
                }
            }

            return bounds;
        }
    }
}
