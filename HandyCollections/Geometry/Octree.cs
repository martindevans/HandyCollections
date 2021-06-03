using System.Numerics;
using SwizzleMyVectors.Geometry;

namespace HandyCollections.Geometry
{
    /// <summary>
    /// 3D Space Partitioning Tree
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class Octree<TItem>
        : GeometricTree<TItem, Vector3, BoundingBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="threshold"></param>
        /// <param name="maxDepth"></param>
        public Octree(BoundingBox bounds, int threshold, int maxDepth)
            : base(bounds, threshold, maxDepth)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="contained"></param>
        /// <returns></returns>
        protected override bool Contains(BoundingBox container, ref BoundingBox contained)
        {
            container.Contains(ref contained, out var result);
            return result == ContainmentType.Contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override bool Intersects(BoundingBox a, ref BoundingBox b)
        {
            a.Intersects(ref b, out var result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <returns></returns>
        protected override BoundingBox[] Split(BoundingBox bound)
        {
            var bounds = new BoundingBox[8];
            var min = bound.Min;
            var size = (bound.Max - bound.Min) / 2f;

            var i = 0;
            for (var x = 0; x < 2; x++)
            {
                for (var y = 0; y < 2; y++)
                {
                    for (var z = 0; z < 2; z++)
                    {
                        var positionOffset = size * new Vector3(x, y, z);
                        bounds[i++] = new BoundingBox(min + positionOffset, min + size + positionOffset);
                    }
                }
            }

            return bounds;
        }
    }
}
