using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandyCollections
{
    /// <summary>
    /// A threadsafe static random number generator
    /// </summary>
    public class StaticRandomNumber
    {
        #region random number generation
        const double REAL_UNIT_INT = 1.0 / ((double)int.MaxValue + 1.0);
        const uint U = 273326509 >> 19;

        /// <summary>
        /// Creates a random number from the specified seed
        /// </summary>
        /// <param name="seed">The seed value</param>
        /// <param name="upperBound">The maximum value (exclusive)</param>
        /// <returns></returns>
        public static uint Random(uint seed, uint upperBound)
        {
            uint t = (seed ^ (seed << 11));
            uint w = 273326509;
            long i = (int)(0x7FFFFFFF & ((w ^ U) ^ (t ^ (t >> 8))));
            return (uint)(i % upperBound);
        }
        #endregion

        /// <summary>
        /// Creates a random number, using the time as a seed
        /// </summary>
        /// <param name="upperBound">The maximum value (exclusive)</param>
        /// <returns></returns>
        public unsafe static uint Random(uint upperBound)
        {
            int time = DateTime.Now.Millisecond;
            uint uTime = *((uint*)&time);

            return Random(uTime, upperBound);
        }

        /// <summary>
        /// Creates a random number, using the time as the seed
        /// </summary>
        /// <returns></returns>
        public unsafe static uint Random()
        {
            return Random(uint.MaxValue);
        }
    }
}
