
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
            BoundingRectangle[] bounds = new BoundingRectangle[4];
            var min = bound.Min;
            var size = (bound.Max - bound.Min) / 2f;

            int i = 0;
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    var positionOffset = size * new Vector2(x, y);
                    bounds[i++] = new BoundingRectangle(min + positionOffset, min + size + positionOffset);
                }
            }

            return bounds;
        }
    }
}
