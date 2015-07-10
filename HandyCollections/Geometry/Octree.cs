
namespace HandyCollections.Geometry
{
    public class Octree<TItem>
        : GeometricTree<TItem, Vector3, BoundingBox>
    {
        public Octree(BoundingBox bounds, int threshold)
            : base(bounds, threshold)
        {
        }

        protected override bool Contains(BoundingBox container, ref BoundingBox contained)
        {
            return container.Contains(ref contained);
        }

        protected override bool Intersects(BoundingBox a, ref BoundingBox b)
        {
            return a.Intersects(ref b);
        }

        protected override BoundingBox[] Split(BoundingBox bound)
        {
            var bounds = new BoundingBox[8];
            var min = bound.Min;
            var size = (bound.Max - bound.Min) / 2f;

            int i = 0;
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 2; z++)
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
