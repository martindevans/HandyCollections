
using System.Diagnostics.Contracts;
using System.Numerics;

namespace HandyCollections.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public struct BoundingBox
    {
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Min;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Max;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public BoundingBox(Vector3 min, Vector3 max)
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
        public bool Intersects(BoundingBox bounds)
        {
            return Intersects(ref bounds);
        }

        [Pure]
        public bool Intersects(ref BoundingBox bounds)
        {
            return bounds.Min.X < Max.X && bounds.Max.X > Min.X
                   && bounds.Min.Y < Max.Y && bounds.Max.Y > Min.Y
                   && bounds.Min.Z < Max.Z && bounds.Max.Z > Min.Z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        [Pure]
        public bool Contains(BoundingBox bounds)
        {
            return Contains(ref bounds);
        }

        [Pure]
        public bool Contains(ref BoundingBox bounds)
        {
            return bounds.Min.X > Min.X && bounds.Max.X < Max.X
                   && bounds.Min.Y > Min.Y && bounds.Max.Y < Max.Y
                   && bounds.Min.Z > Min.Z && bounds.Max.Z < Max.Z;
        }
    }
}
