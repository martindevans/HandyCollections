using System.Diagnostics.Contracts;
using System.Numerics;

namespace HandyCollections.Geometry
{
    public struct BoundingRectangle
    {
        /// <summary>
        /// 
        /// </summary>
        public Vector2 Min;

        /// <summary>
        /// 
        /// </summary>
        public Vector2 Max;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public BoundingRectangle(Vector2 min, Vector2 max)
        {
            Max = max;
            Min = min;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        [Pure]
        public bool Intersects(BoundingRectangle bounds)
        {
            return bounds.Min.X < Max.X && bounds.Max.X > Min.X
                && bounds.Min.Y < Max.Y && bounds.Max.Y > Min.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        [Pure]
        public bool Contains(BoundingRectangle bounds)
        {
            return bounds.Min.X > Min.X && bounds.Max.X < Max.X
                && bounds.Min.Y > Min.Y && bounds.Max.Y < Max.Y;
        }
    }
}
