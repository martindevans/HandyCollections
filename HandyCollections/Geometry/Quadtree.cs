
using System.Numerics;
using SwizzleMyVectors.Geometry;

namespace HandyCollections.Geometry
{
    public class Quadtree<TItem>
        : GeometricTree<TItem, Vector2, BoundingRectangle>
    {
        public Quadtree(BoundingRectangle bounds, int threshold)
            : base(bounds, threshold)
        {
        }

        protected override bool Contains(BoundingRectangle container, ref BoundingRectangle contained)
        {
            return container.Contains(contained);
        }

        protected override bool Intersects(BoundingRectangle a, ref BoundingRectangle b)
        {
            return a.Intersects(b);
        }

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
